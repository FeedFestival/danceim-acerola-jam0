using Cinemachine;
using DG.Tweening;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player {
    public class CameraController : MonoBehaviour, ICameraController {

        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private float _sensitivity = 2f;
        [SerializeField]
        private float _minPitch = -60f;
        [SerializeField]
        private float _maxPitch = 82f;
        [SerializeField]
        private CinemachineVirtualCamera _cinemachineVirtualCamera;
        private CinemachineComposer _aim;
        private Cinemachine3rdPersonFollow _body;
        private CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

        private IGameplayState _gameplayStateRef;

        [SerializeField]
        private Transform _virtualTargetLookAt;
        [SerializeField]
        private Vector3 _offsetPos = new Vector3(0f, 1.85f, 0f);

        [SerializeField]
        private CameraSettingsSO _cameraSettings;

        [Header("Head Bob")]
        [SerializeField]
        private NoiseSettings _vcamNoiseIdle;
        [SerializeField]
        private NoiseSettings _vcamNoiseWalk;
        [SerializeField]
        private NoiseSettings _vcamNoiseRun;
        private Tweener _cameraNoiseAmplitude;

        private Transform _actorT;
        private Transform _spineToOrientate;

        private float _relativeYaw = 0f;
        private float _pitch = 0f;
        private float _yaw = 0f;
        private Vector2 _mouseLook;

        private bool _defaultWasSet = false;
        private float _absoluteMinPitch;
        private readonly float _relativeMaxPitch = 106;
        private float _absoluteMaxPitch;
        private IFocusTrigger _lastFocusedTrigger;
        private string _lastMinigameKey;

        private readonly float _fov_min = 82f;
        private readonly float _fov_max = 72f;
        private Vector3 _damping_min = Vector3.zero;
        private Vector3 _damping_max = new Vector3(0.05f, 0.05f, 0.05f);
        private IEnumerator _waitCheckLookInteractable;
        private IEnumerator _waitCheckMouseInteractable;
        private readonly float _lookaheadTime_min = 0f;
        private readonly float _lookaheadTime_max = 0.5f;
        private readonly float _lookaheadSmoothing_min = 0f;
        private readonly float _lookaheadSmoothing_max = 50;
        private readonly float _aimmScreenY_min = 0;
        private readonly float _aimmScreenY_max = 0.5f;
        private readonly float _aimmSoftZoneSizeMin = .0f;
        private readonly float _aimmSoftZoneSizeMax = .8f;

        private Tweener _restoreLocalPosition;
        private int _interactableLayerMask;
        private readonly float _checkInteractRange = 3f;
        private float _waitCheckTime;

        //---------------------------------------------------------------------------------------------

        public Camera Camera { get => _camera; }
        public float RelativeYaw { get => _relativeYaw; }
        public Transform Transform { get => transform; }
        public Action<int?> OnCameraFocussedInteractable { get; set; }

        public void Init(IGameplayState gameplayState, IUnit unitRef) {

            _gameplayStateRef = gameplayState;
            _gameplayStateRef.OnGameplayRecalculation += recalculation;
            recalculation();

            _actorT = unitRef.Transform;
            _spineToOrientate = (unitRef as IPlayerUnit).SpineToOrientate;

            _absoluteMinPitch = 0 - _minPitch;
            _absoluteMaxPitch = (_maxPitch + _absoluteMinPitch) - _relativeMaxPitch;

            _aim = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineComposer>();
            _body = _cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            _interactableLayerMask = LayerMask.GetMask("INTERACTABLE");

            _multiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            SetCameraNoise(MotorState.Idle);
        }

        public void SetLookPosition(Vector2 mouseLook) {
            _mouseLook = mouseLook;
        }

        public IFocusTrigger GetFocusedTrigger() {
            return _lastFocusedTrigger;
        }

        public void SetCameraNoise(MotorState motorState) {

            if (_cameraNoiseAmplitude != null) {
                _cameraNoiseAmplitude.Kill();
            }

            _cameraNoiseAmplitude = DOVirtual.Float(1, 0, 0.33f, (float value) => {
                _multiChannelPerlin.m_AmplitudeGain = value;
            })
                .OnComplete(() => {
                    switch (motorState) {
                        case MotorState.Idle:
                            _multiChannelPerlin.m_NoiseProfile = _vcamNoiseIdle;
                            break;
                        case MotorState.Moving:
                            _multiChannelPerlin.m_NoiseProfile = _vcamNoiseWalk;
                            break;
                        case MotorState.Sprinting:
                            _multiChannelPerlin.m_NoiseProfile = _vcamNoiseRun;
                            break;
                        case MotorState.Crouching:
                            break;
                        case MotorState.None:
                        default:
                            _multiChannelPerlin.m_NoiseProfile = null;
                            break;
                    }

                    _cameraNoiseAmplitude = DOVirtual.Float(0, 1, 0.33f, (float value) => {
                        _multiChannelPerlin.m_AmplitudeGain = value;
                    });
                });
        }

        public void SetVirtualCameraFocusTarget(
            Vector3? futurePos,
            Transform focusTarget = null
            ) {

            if (focusTarget == null) {
                if (_restoreLocalPosition != null) {
                    _restoreLocalPosition.Kill();
                }
                _restoreLocalPosition = DOVirtual
                    .Vector3(_virtualTargetLookAt.localPosition, new Vector3(0, 0, 4), 2f, (Vector3 value) => {
                        _virtualTargetLookAt.localPosition = value;
                    })
                    .SetEase(Ease.InSine)
                    ;
                _mouseLook = Vector2.zero;
                return;
            }

            if (_restoreLocalPosition != null) {
                _restoreLocalPosition.Kill();
            }

            var pos = new Vector3(futurePos.Value.x, transform.position.y, futurePos.Value.z);
            var rotation = Quaternion.LookRotation((focusTarget.position - pos).normalized);
            transform.DORotateQuaternion(rotation, 0.33f)
                .SetEase(Ease.InSine)
                .OnComplete(() => {
                    _pitch = rotation.eulerAngles.x;
                    _yaw = rotation.eulerAngles.y;

                    _virtualTargetLookAt.DOMove(focusTarget.position, 0.33f)
                        .SetEase(Ease.InSine)
                        ;
                });
        }

        public (Vector3 origin, Vector3 direction, int? id) GetAimHitUnit() {
            var ray = new Ray(_cinemachineVirtualCamera.transform.position, _cinemachineVirtualCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit, _checkInteractRange, _interactableLayerMask)) {
                var focusTrigger = hit.transform.GetComponent<IFocusTrigger>();
                if (focusTrigger != null && focusTrigger.Type == InteractType.Unit) {
                    return (ray.origin, ray.direction, focusTrigger.ID);
                }
                Debug.DrawLine(ray.origin, hit.point, Color.green, _waitCheckTime);
            }

            var endPosition = ray.origin + ray.direction * _checkInteractRange;
            Debug.DrawLine(ray.origin, endPosition, Color.white, _waitCheckTime);

            return (ray.origin, ray.direction, null);
        }

        //---------------------------------------------------------------------------------------------------

        void LateUpdate() {

            if (_gameplayStateRef.CameraControlPermissions[CameraControl.Position] == true) {
                _spineToOrientate.rotation = transform.rotation;

                transform.position = new Vector3(
                    _actorT.position.x + _offsetPos.x,
                    _actorT.position.y + _offsetPos.y,
                    _actorT.position.z + _offsetPos.z
                );
            }


            if (_gameplayStateRef.CameraControlPermissions[CameraControl.Look] == true) {
                cameraRotate();
                changeCameraSettings();
            }
        }

        void cameraRotate() {
            _relativeYaw = _mouseLook.x * _sensitivity;
            _pitch -= _mouseLook.y * _sensitivity;
            _yaw += _mouseLook.x * _sensitivity;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
            transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
        }

        private void changeCameraSettings() {

            var currentPitch = _pitch + _absoluteMinPitch;
            if (currentPitch < _relativeMaxPitch) {
                setDefaultCameraSettings();
                return;
            }

            _defaultWasSet = false;

            var c = currentPitch - _relativeMaxPitch;
            var p = 1f - (float)Math.Round(c / _absoluteMaxPitch, 2);

            setCameraSettings(p);
        }

        private void setDefaultCameraSettings() {
            if (_defaultWasSet) {
                return;
            }

            _defaultWasSet = true;

            setCameraSettings(1);
        }

        private void setCameraSettings(float p) {

            _cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Max(Mathf.Lerp(_fov_min, _fov_max, p), 0);

            if (_cameraSettings.LerpShoulderOffset) {
                _body.ShoulderOffset = Vector3.Lerp(_cameraSettings.ShoulderOffsetMin, _cameraSettings.ShoulderOffsetMax, p);
                _body.Damping = Vector3.Lerp(_damping_min, _damping_max, p);
            }

            _aim.m_LookaheadTime = Mathf.Max(Mathf.Lerp(_lookaheadTime_min, _lookaheadTime_max, p), 0);
            _aim.m_LookaheadSmoothing = Mathf.Max(Mathf.Lerp(_lookaheadSmoothing_min, _lookaheadSmoothing_max, p), 0);

            _aim.m_ScreenY = Mathf.Max(Mathf.Lerp(_aimmScreenY_min, _aimmScreenY_max, p), 0);

            _aim.m_SoftZoneWidth = Mathf.Max(Mathf.Lerp(_aimmSoftZoneSizeMin, _aimmSoftZoneSizeMax, p), 0);
            _aim.m_SoftZoneHeight = Mathf.Max(Mathf.Lerp(_aimmSoftZoneSizeMin, _aimmSoftZoneSizeMax, p), 0);

            if (p < 1) {
                _aim.m_HorizontalDamping = 0;
                _aim.m_VerticalDamping = 0;

            } else {
                _aim.m_HorizontalDamping = 0.5f;
                _aim.m_VerticalDamping = 0.1f;
            }
        }

        private void checkLookInteractable() {
            tryStopCheckLook();

            var ray = new Ray(_cinemachineVirtualCamera.transform.position, _cinemachineVirtualCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit, _checkInteractRange, _interactableLayerMask)) {
                onHit(ray, hit);
            } else {
                onMiss(ray);
            }

            _waitCheckLookInteractable = waitCheckInteractable();
            StartCoroutine(_waitCheckLookInteractable);
        }

        private void checkMousePosInteractable() {
            tryStopCheckMousePos();

            var ray = _camera.ScreenPointToRay(_mouseLook);
            if (Physics.Raycast(ray, out var hit, _checkInteractRange, _interactableLayerMask)) {
                var miniGamefocusTrigger = hit.transform.GetComponent<IMinigameFocusTrigger>();
                if (miniGamefocusTrigger != null) {
                    if (_lastMinigameKey != miniGamefocusTrigger.Key) {
                        _lastMinigameKey = miniGamefocusTrigger.Key;
                        miniGamefocusTrigger.SetIsFocused();
                    }
                }
                Debug.DrawLine(ray.origin, hit.point, Color.green, _waitCheckTime);
            } else {
                Debug.DrawLine(ray.origin, hit.point, Color.red, _waitCheckTime);
            }

            _waitCheckLookInteractable = waitCheckMouseInteractable();
            StartCoroutine(_waitCheckLookInteractable);
        }

        private void onHit(Ray ray, RaycastHit hit) {
            var focusTrigger = hit.transform.GetComponent<IFocusTrigger>();
            if (focusTrigger != null) {
                if (_lastFocusedTrigger != null && _lastFocusedTrigger.ID != focusTrigger.ID) {
                    _lastFocusedTrigger.SetIsFocused(false);
                    Debug.DrawLine(ray.origin, hit.point, Color.green, _waitCheckTime);
                }
                if (_lastFocusedTrigger == null || _lastFocusedTrigger.ID != focusTrigger.ID) {
                    _lastFocusedTrigger = focusTrigger;
                    _lastFocusedTrigger.SetIsFocused();
                    OnCameraFocussedInteractable?.Invoke(_lastFocusedTrigger.ID);
                }
            }
            Debug.DrawLine(ray.origin, hit.point, Color.red, _waitCheckTime);
        }

        private void onMiss(Ray ray) {
            if (_lastFocusedTrigger != null) {
                _lastFocusedTrigger.SetIsFocused(false);
                _lastFocusedTrigger = null;
                OnCameraFocussedInteractable?.Invoke(null);
            }
            var endPosition = ray.origin + ray.direction * _checkInteractRange;
            Debug.DrawLine(ray.origin, endPosition, Color.white, _waitCheckTime);
        }

        private IEnumerator waitCheckInteractable() {
            yield return new WaitForSeconds(_waitCheckTime);
            checkLookInteractable();
        }

        private void tryStopCheckLook() {
            if (_waitCheckLookInteractable != null) {
                StopCoroutine(_waitCheckLookInteractable);
                _waitCheckLookInteractable = null;
            }
        }

        private IEnumerator waitCheckMouseInteractable() {
            yield return new WaitForSeconds(_waitCheckTime);
            checkMousePosInteractable();
        }

        private void tryStopCheckMousePos() {
            if (_waitCheckMouseInteractable != null) {
                StopCoroutine(_waitCheckMouseInteractable);
                _waitCheckMouseInteractable = null;
            }
        }

        private void recalculation() {
            if (_gameplayStateRef.CameraControlPermissions[CameraControl.Look]) {
                _waitCheckTime = 0.25f;
                checkLookInteractable();
            } else {
                tryStopCheckLook();
            }

            if (_gameplayStateRef.CameraControlPermissions[CameraControl.Mouse]) {
                _waitCheckTime = 0.1f;
                checkMousePosInteractable();
            } else {
                tryStopCheckMousePos();
            }
        }
    }
}