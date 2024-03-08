using Game.Shared.Bus;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player {
    public class PlayerControl : MonoBehaviour {

        private IPlayerUnitControl _unitControlRef;
        private IPlayer _playerRef;
        private IUI _uIRef;
        private InputManager _input;
        private Dictionary<ControlPermission, bool> _controlPermissions = new Dictionary<ControlPermission, bool>() {
            {
                ControlPermission.ControlGlobal,
                false
            },
            {
                ControlPermission.ControlLook,
                false
            },
            {
                ControlPermission.ControlMovement,
                false
            }
        };

        internal void Init(IPlayer player) {

            _playerRef = player;
            _unitControlRef = player.Unit.UnitControl as IPlayerUnitControl;
            _unitControlRef.Init(player.CameraController, player.Unit.Actor);
            _uIRef = player.UI;

            _input = new InputManager();

            _input.Enable();

            _input.PlayerLook.Look.performed += lookPerformed;
            _input.PlayerLook.Look.canceled += lookCanceled;

            _input.Player.Movement.performed += movementPerformed;
            _input.Player.Movement.canceled += movementCanceled;
            _input.Player.Sprint.performed += sprintPerformed;
            _input.Player.Jump.performed += jumpPerformed;

            _input.Global.PauseMenu.performed += pauseMenuPerformed;
            _input.Global.Interact.performed += interactPerformed;

            _unitControlRef.AnalogControl(false);
        }

        public void EnableControlPermission(ControlPermission controlPermission, bool enabled = true) {

            if (_controlPermissions[controlPermission] == enabled) {
                return;
            }
            _controlPermissions[controlPermission] = enabled;

            switch (controlPermission) {
                case ControlPermission.ControlMovement:

                    if (_controlPermissions[controlPermission]) {
                        _input.Player.Enable();
                        _uIRef.SetContextAction(UIContextAction.DefaultCrosshair);
                    } else {
                        _input.Player.Disable();
                        _uIRef.SetContextAction(UIContextAction.None);
                    }

                    break;
                case ControlPermission.ControlLook:

                    if (_controlPermissions[controlPermission]) {
                        _input.PlayerLook.Enable();
                        _uIRef.SetContextAction(UIContextAction.DefaultCrosshair);
                    } else {
                        _input.PlayerLook.Disable();
                        _uIRef.SetContextAction(UIContextAction.None);
                    }

                    break;
                case ControlPermission.ControlGlobal:
                default:

                    if (_controlPermissions[controlPermission]) {
                        _input.Global.Enable();
                        _uIRef.SetContextAction(UIContextAction.DefaultCrosshair);
                    } else {
                        _input.Global.Disable();
                        _uIRef.SetContextAction(UIContextAction.None);
                    }

                    break;
            }
        }

        //---------------------------------------------------------------------------------------------------------

        private void jumpPerformed(InputAction.CallbackContext context) {
            var jumpPressed = context.ReadValueAsButton();

            Debug.Log("jumpPressed: " + jumpPressed);
        }

        private void movementPerformed(InputAction.CallbackContext context) {
            var position = context.ReadValue<Vector2>();
            _unitControlRef.Move(position);
        }

        private void movementCanceled(InputAction.CallbackContext context) {
            var position = context.ReadValue<Vector2>();
            _unitControlRef.Move(position);
        }

        private void lookPerformed(InputAction.CallbackContext context) {
            (_playerRef.CameraController as CameraController).SetLookPosition(context.ReadValue<Vector2>());
        }

        private void lookCanceled(InputAction.CallbackContext context) {
            (_playerRef.CameraController as CameraController).SetLookPosition(context.ReadValue<Vector2>());
        }

        private void sprintPerformed(InputAction.CallbackContext context) {
            var sprintPressed = context.ReadValueAsButton();
            _unitControlRef.Sprint(sprintPressed);
        }

        private void pauseMenuPerformed(InputAction.CallbackContext context) {
            var pauseMenuPressed = context.ReadValueAsButton();
            if (!pauseMenuPressed) {
                return;
            }

            if (_playerRef.PlayerState == PlayerState.BrowsingMenu) {
                _playerRef.SetControlState(PlayerState.Playing); // we should maybe reinstate the old state
                _uIRef.Open(InGameMenu.Pause, false);
            } else {
                _playerRef.SetControlState(PlayerState.BrowsingMenu);
                _uIRef.Open(InGameMenu.Pause);
            }
        }

        private void interactPerformed(InputAction.CallbackContext context) {
            var interactPressed = context.ReadValueAsButton();
            if (!interactPressed) {
                return;
            }

            if (_uIRef.UIContextAction == UIContextAction.DefaultAction) {
                __.GameBus.Emit(GameEvt.PLAYER_INTERACTED, _playerRef.CameraController.GetFocusedId());
            }
        }
    }
}
