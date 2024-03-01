using Cinemachine;
using DG.Tweening;
using Game.Shared.Interfaces;
using System;
using System.Collections;
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

        [SerializeField]
        private Transform _virtualTargetLookAt;
        [SerializeField]
        private Vector3 _offsetPos = new Vector3(0f, 1.85f, 0f);

        private Transform _actorT;
        private Transform _spineToOrientate;

        private bool _enabled;

        private float _relativeYaw = 0f;
        private float _pitch = 0f;
        private float _yaw = 0f;
        private Vector2 _mouseLook;

        private bool _defaultWasSet = false;
        private float _absoluteMinPitch;
        private readonly float _relativeMaxPitch = 106;
        private float _absoluteMaxPitch;
        private int? _focusedId;

        private readonly float _fov_min = 82f;
        private readonly float _fov_max = 72f;
        private Vector3 _shoulderOffset_min = new Vector3(0, 0.12f, 0.44f);
        private Vector3 _shoulderOffset_max = new Vector3(0, 0.12f, 0.12f);
        private Vector3 _damping_min = Vector3.zero;
        private Vector3 _damping_max = new Vector3(0.05f, 0.05f, 0.05f);
        private IEnumerator _waitCheckInteractable;
        private readonly float _lookaheadTime_min = 0f;
        private readonly float _lookaheadTime_max = 0.5f;
        private readonly float _lookaheadSmoothing_min = 0f;
        private readonly float _lookaheadSmoothing_max = 20;
        private readonly float _aimmScreenY_min = 0;
        private readonly float _aimmScreenY_max = 0.5f;
        private readonly float _aimmSoftZoneSizeMin = .0f;
        private readonly float _aimmSoftZoneSizeMax = .8f;

        private Tweener _restoreLocalPosition;
        private int _interactableLayerMask;
        private readonly float _checkInteractRange = 3f;

        public float RelativeYaw { get => _relativeYaw; }
        public Transform Transform { get => transform; }
        public Action<int?> OnCameraFocussedInteractable { get; set; }

        public void Init(IUnit unitRef) {

            _actorT = unitRef.Transform;
            _spineToOrientate = unitRef.SpineToOrientate;

            _absoluteMinPitch = 0 - _minPitch;
            _absoluteMaxPitch = (_maxPitch + _absoluteMinPitch) - _relativeMaxPitch;

            _aim = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineComposer>();
            _body = _cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            _interactableLayerMask = LayerMask.GetMask("INTERACTABLE");
            checkInteractable();
        }

        public void SetLookPosition(Vector2 mouseLook) {
            _mouseLook = mouseLook;
        }

        public void Enable(bool enable = true) {
            _enabled = enable;
        }

        public int GetFocusedId() {
            return _focusedId.Value;
        }

        public void SetVirtualCameraFocusTarget(
            Vector3? futurePos,
            Transform focusTarget = null
            ) {

            if (focusTarget == null) {
                if (_restoreLocalPosition != null) {
                    _restoreLocalPosition.Kill();
                }
                _restoreLocalPosition = DOVirtual.Vector3(_virtualTargetLookAt.localPosition, new Vector3(0, 0, 4), 2f, (Vector3 value) => {
                    _virtualTargetLookAt.localPosition = value;
                }).SetEase(Ease.InSine);
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
                        .SetEase(Ease.InSine);
                });
        }

        //---------------------------------------------------------------------------------------------------

        void LateUpdate() {

            _spineToOrientate.rotation = transform.rotation;

            if (_enabled) {
                cameraRotate();
            }

            transform.position = new Vector3(
                _actorT.position.x + _offsetPos.x,
                _actorT.position.y + _offsetPos.y,
                _actorT.position.z + _offsetPos.z
            );

            changeCameraSettings();
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

            _body.ShoulderOffset = Vector3.Lerp(_shoulderOffset_min, _shoulderOffset_max, p);
            _body.Damping = Vector3.Lerp(_damping_min, _damping_max, p);

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

        private void checkInteractable() {
            if (_waitCheckInteractable != null) {
                StopCoroutine(_waitCheckInteractable);
                _waitCheckInteractable = null;
            }

            var ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, _checkInteractRange, _interactableLayerMask)) {
                var focusTrigger = hit.transform.GetComponent<IFocusTrigger>();
                if (focusTrigger != null) {
                    if (!_focusedId.HasValue || _focusedId.Value != focusTrigger.ID) {
                        _focusedId = focusTrigger.ID;
                        OnCameraFocussedInteractable?.Invoke(_focusedId);
                    }
                }
            } else {
                if (_focusedId.HasValue) {
                    _focusedId = null;
                    OnCameraFocussedInteractable?.Invoke(_focusedId);
                }
            }

            _waitCheckInteractable = waitCheckInteractable();
            StartCoroutine(_waitCheckInteractable);
        }

        private IEnumerator waitCheckInteractable() {
            yield return new WaitForSeconds(0.25f);
            checkInteractable();
        }
    }
}