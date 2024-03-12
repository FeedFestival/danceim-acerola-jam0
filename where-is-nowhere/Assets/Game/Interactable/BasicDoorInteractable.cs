using DG.Tweening;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Interactable {
    public class BasicDoorInteractable : BaseInteractable, IRequiredItems {

        [Header("Basic Door Interactable")]
        [SerializeField]
        private Transform _teleportPointT;
        [SerializeField]
        private Transform _backTeleportPointT;
        [SerializeField]
        private Transform _focusPointT;
        [SerializeField]
        private Transform _gateT;
        [SerializeField]
        private NavMeshObstacle _navMeshObstacle;

        [SerializeField]
        private bool _isOpen;

        [SerializeField]
        private float _doorMaxRot = 100f;
        [SerializeField]
        private float _doorOpenAnimation = 3.00f;
        [SerializeField]
        private RotateMode _rotateMode;
        [SerializeField]
        private bool _invertFront;
        private Vector3 _originalRot;

        public InventoryItem[] RequiredItems { get; set; }

        public override void Init() {
            base.initEntityId();
            base.initFocusTrigger();

            _originalRot = _gateT.transform.eulerAngles;
        }

        public override void DoDefaultInteraction(IPlayer player) {

            if (_isOpen == false) {
                openDoor(player);
            } else {
                closeDoor(player);
            }
        }

        private void openDoor(IPlayer player = null) {

            var unitInFront = isUnitInFront(player.Unit.Transform.position);
            var teleportPoint = unitInFront
                ? _teleportPointT.position
                : _backTeleportPointT.position;

            player.Unit.UnitControl.Teleport(teleportPoint, smooth: true);

            player.GameplayState.SetState(
                GameState.InGame,
                PlayerState.Interacting,
                UnitState.Interacting
            );

            player.CameraController.SetVirtualCameraFocusTarget(
                futurePos: teleportPoint,
                _focusPointT
            );

            var toRot = new Vector3(_originalRot.x, _doorMaxRot, _originalRot.z);
            _focusTrigger.Enable(false);
            base.onFocused(false);
            _navMeshObstacle.carving = false;

            _gateT.DORotate(toRot, _doorOpenAnimation, _rotateMode)
                //.SetEase(Ease.InSine)
                .OnComplete(() => {

                    player.GameplayState.SetState(
                        GameState.InGame,
                        PlayerState.Playing,
                        UnitState.FreePlaying
                    );
                    player.CameraController.SetVirtualCameraFocusTarget();

                    _focusTrigger.Enable();
                    _navMeshObstacle.carving = true;

                    _isOpen = true;
                });
        }

        private void closeDoor(IPlayer player = null) {

            var unitInFront = isUnitInFront(player.Unit.Transform.position);
            var teleportPoint = unitInFront
                ? _teleportPointT.position
                : _backTeleportPointT.position;

            player.Unit.UnitControl.Teleport(teleportPoint, smooth: true);

            player.GameplayState.SetState(
                GameState.InGame,
                PlayerState.Interacting,
                UnitState.Interacting
            );
            player.CameraController.SetVirtualCameraFocusTarget(
                futurePos: teleportPoint,
                _focusPointT
            );

            var toRot = new Vector3(_originalRot.x, -_doorMaxRot, _originalRot.z);
            _focusTrigger.Enable(false);
            _navMeshObstacle.carving = false;

            _gateT.DORotate(toRot, _doorOpenAnimation, _rotateMode)
                //.SetEase(Ease.InSine)
                .OnComplete(() => {

                    player.GameplayState.SetState(
                        GameState.InGame,
                        PlayerState.Playing,
                        UnitState.FreePlaying
                    );
                    player.CameraController.SetVirtualCameraFocusTarget();

                    _focusTrigger.Enable();
                    _navMeshObstacle.carving = true;

                    _isOpen = false;
                });
        }

        private bool isUnitInFront(Vector3 unitPosition) {

            var directionToPosition = unitPosition - transform.position;
            float dotProduct = Vector3.Dot(directionToPosition.normalized, transform.forward.normalized);

            if (dotProduct > 0) {
                return _invertFront ? false : true;
            }
            return _invertFront ? true : false;
        }
    }
}