using Game.Shared.Bus;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player {
    public class PlayerControl : MonoBehaviour, IPlayerControl {

        private IPlayerUnitControl _unitControlRef;
        private IPlayer _playerRef;
        private IUI _uIRef;
        private InputManager _input;
        private InteractionType _interactionType;
        private bool _captureMousePosition;
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

        public Action FirePerformed { get; set; }

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
            _input.Player.Fire.performed += firePerformed;

            _input.Global.PauseMenu.performed += pauseMenuPerformed;
            _input.Global.Interact.performed += interactPerformed;

            _unitControlRef.AnalogControl(false);
        }

        public void SetInteractionControl(InteractionType interactionType) {
            if (_interactionType == interactionType) {
                return;
            }
            _interactionType = interactionType;

            Debug.Log("_interactionType: " + _interactionType);

            switch (_interactionType) {
                case InteractionType.None:
                    break;
                case InteractionType.WorldSelection:

                    //_input.Player.Enable();
                    //_input.Player.Movement.Disable();
                    //_input.Player.Sprint.Disable();
                    //_input.Player.Jump.Disable();

                    _input.Player.Fire.Enable();

                    _input.PlayerLook.Look.performed -= lookPerformed;
                    _input.PlayerLook.Look.canceled -= lookCanceled;

                    (_playerRef.CameraController as CameraController).SetCameraControl(CameraControl.Look, false);
                    (_playerRef.CameraController as CameraController).SetCameraControl(CameraControl.Mouse);

                    _captureMousePosition = true;

                    _uIRef.SetContextAction(UIContextAction.MovingCrosshair);

                    break;
                case InteractionType.UISelection:

                    break;
                case InteractionType.Default:
                default:

                    _input.PlayerLook.Look.performed += lookPerformed;
                    _input.PlayerLook.Look.canceled += lookCanceled;
                    _captureMousePosition = false;

                    break;
            }
        }

        public void EnableControlPermission(ControlPermission controlPermission, bool enabled = true) {

            if (_controlPermissions[controlPermission] == enabled) {
                return;
            }
            _controlPermissions[controlPermission] = enabled;
            Debug.Log("controlPermission: " + controlPermission + " = " + enabled);

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

        private void FixedUpdate() {
            if (_captureMousePosition == false) { return; }

            (_playerRef.CameraController as CameraController).SetLookPosition(Input.mousePosition);
            var mousePosition = new Vector2(Input.mousePosition.x - (Screen.width / 2), Input.mousePosition.y - (Screen.height / 2));
            _uIRef.SetMousePosition(mousePosition);
        }

        private void jumpPerformed(InputAction.CallbackContext context) {
            var jumpPressed = context.ReadValueAsButton();
        }

        private void firePerformed(InputAction.CallbackContext context) {
            var firePressed = context.ReadValueAsButton();
            if (!firePressed) { return; }

            FirePerformed?.Invoke();
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
