using DG.Tweening;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Interactable {
    public class DrawerInteractable : BaseInteractable, IRequiredItems {

        [Header("Drawer Interactable")]
        [SerializeField]
        private Transform _teleportPointT;
        [SerializeField]
        private Transform _focusPointT;
        [SerializeField]
        private Transform _doorT;

        [SerializeField]
        private bool _isOpen;

        [SerializeField]
        private float _doorOpenAnimation = 3.00f;
        [SerializeField]
        private RotateMode _rotateMode;
        [SerializeField]
        private bool _invertFront;
        [SerializeField]
        private float _rotateAmount = 100;
        private Vector3 _originalRot;

        public InventoryItem[] RequiredItems { get; set; }

        public override void Init() {
            base.initEntityId();
            base.initFocusTrigger();

            _originalRot = _doorT.transform.eulerAngles;
        }

        public override void DoDefaultInteraction(IPlayer player) {

            if (_isOpen == false) {
                openDoor(player);
            } else {
                closeDoor(player);
            }
        }

        private void openDoor(IPlayer player = null) {

            //player.Unit.UnitControl.Teleport(_teleportPointT.position, smooth: true);

            //player.GameplayState.SetState(
            //    GameState.InGame,
            //    PlayerState.Interacting,
            //    UnitState.Interacting
            //);

            //player.CameraController.SetVirtualCameraFocusTarget(
            //    futurePos: _teleportPointT.position,
            //    _focusPointT
            //);

            var toRot = new Vector3(_originalRot.x, _rotateAmount, _originalRot.z);
            _focusTrigger.Enable(false);
            base.onFocused(false);

            _doorT.DORotate(toRot, _doorOpenAnimation, _rotateMode)
                //.SetEase(Ease.InSine)
                .OnComplete(() => {

                    //player.GameplayState.SetState(
                    //    GameState.InGame,
                    //    PlayerState.Playing,
                    //    UnitState.FreePlaying
                    //);
                    //player.CameraController.SetVirtualCameraFocusTarget();

                    _focusTrigger.Enable();

                    _isOpen = true;
                });
        }

        private void closeDoor(IPlayer player = null) {

            //player.Unit.UnitControl.Teleport(_teleportPointT.position, smooth: true);

            //player.GameplayState.SetState(
            //    GameState.InGame,
            //    PlayerState.Interacting,
            //    UnitState.Interacting
            //);
            //player.CameraController.SetVirtualCameraFocusTarget(
            //    futurePos: _teleportPointT.position,
            //    _focusPointT
            //);

            var toRot = new Vector3(_originalRot.x, -_rotateAmount, _originalRot.z);
            _focusTrigger.Enable(false);

            _doorT.DORotate(toRot, _doorOpenAnimation, _rotateMode)
                //.SetEase(Ease.InSine)
                .OnComplete(() => {

                    //player.GameplayState.SetState(
                    //    GameState.InGame,
                    //    PlayerState.Playing,
                    //    UnitState.FreePlaying
                    //);
                    //player.CameraController.SetVirtualCameraFocusTarget();

                    _focusTrigger.Enable();

                    _isOpen = false;
                });
        }
    }
}
