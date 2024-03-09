using Game.Interactable;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using TMPro;
using UnityEngine;

namespace Game.Interactables {
    public class WallKeypadInteractable : BaseInteractable {

        [Header("Wall Keypad Interactable")]
        [SerializeField]
        private Transform _teleportPointT;
        [SerializeField]
        private Transform _focusPointT;
        [SerializeField]
        private WallKeypadKey[] _keys;
        [SerializeField]
        private TMP_Text _screenText;

        private IPlayer _playerRef;

        private string _focussedKey;

        public override void Init() {
            base.initEntityId();
            base.initFocusTrigger();

            initKeys();
            enableKeys(false);

            _screenText.text = "";
        }

        private void OnDestroy() {
            foreach (var key in _keys) {
                key.OnFocussed -= keyFocussed;
            }
        }

        private void initKeys() {
            int i = 0;
            foreach (var key in _keys) {
                i++;
                key.Init(i.ToString());

                key.OnFocussed += keyFocussed;
            }
        }

        public override void DoDefaultInteraction(IPlayer player) {

            _playerRef = player;

            _playerRef.PlayerControl.FirePerformed += keyPressed;

            (_playerRef.Unit.UnitControl as IPlayerUnitControl).Teleport(_teleportPointT.position, smooth: true);

            _playerRef.SetControlState(PlayerState.Interacting);
            _playerRef.SetInteractionControl(InteractionType.WorldSelection);
            _playerRef.CameraController.SetCameraNoise(MotorState.None);
            _playerRef.CameraController.SetVirtualCameraFocusTarget(
                futurePos: _teleportPointT.position,
                _focusPointT
            );

            _focusTrigger.Enable(false);
            base.onFocused(false);



            enableKeys();
        }

        private void enableKeys(bool enabled = true) {
            foreach (var key in _keys) {
                key.Enable(enabled);
            }
        }

        private void keyFocussed(string key) {
            _focussedKey = key;
        }

        private void keyPressed() {

            if (_screenText.text == "CORRECT") {
                return;
            }

            if (_screenText.text == "FAILED") {
                _screenText.text = "";
            }

            _screenText.text += _focussedKey;

            if (_screenText.text.Length == 4) {
                var validCode = "4469";

                if (_screenText.text == validCode) {
                    _screenText.text = "CORRECT";
                } else {
                    _screenText.text = "FAILED";
                }
            }
        }
    }
}