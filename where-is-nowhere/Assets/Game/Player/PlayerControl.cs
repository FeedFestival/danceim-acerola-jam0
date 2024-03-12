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
        private bool _captureMousePosition;
        private IGameplayState _gameplayStateRef;

        public Action FirePerformed { get; set; }
        public Action ExitInteraction { get; set; }

        public void Init(IGameplayState gameplayState, IPlayer player) {
            _gameplayStateRef = gameplayState;
            _gameplayStateRef.OnGameplayRecalculation += recalculation;

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

            FirePerformed += _unitControlRef.Fire;

            recalculation();
        }

        public void ResetCrosshair() {
            Debug.Log("ResetCrosshair > ");
            _captureMousePosition = false;
            //_uIRef.SetContextAction();
            //var mousePosition = new Vector2(Screen.width / 2, Screen.height / 2);
            _uIRef.SetMousePosition(Vector3.zero);
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

            if (_playerRef.GameplayState.GameState == GameState.InGame) {

                if (_playerRef.GameplayState.PlayerState == PlayerState.Interacting) {

                    ExitInteraction?.Invoke();

                } else if (_playerRef.GameplayState.PlayerState == PlayerState.Playing) {
                    _playerRef.GameplayState.SetState(
                        GameState.Paused,
                        PlayerState.BrowsingMenu
                    );
                    _uIRef.Open(InGameMenu.Pause);
                }

            } else if (_playerRef.GameplayState.GameState == GameState.Paused) {

                if (_playerRef.GameplayState.PlayerState == PlayerState.BrowsingMenu) {
                    // we should maybe reinstate the old state
                    _playerRef.GameplayState.SetState(
                        GameState.InGame,
                        PlayerState.Playing
                    );
                    _uIRef.Open(InGameMenu.Pause, false);
                }
            }
        }

        private void interactPerformed(InputAction.CallbackContext context) {
            var interactPressed = context.ReadValueAsButton();
            if (!interactPressed) {
                return;
            }

            if (_uIRef.UIContextAction == UIContextAction.DefaultAction) {
                var focusTrigger = _playerRef.CameraController.GetFocusedTrigger();
                if (focusTrigger.Type == InteractType.Interactable) {
                    __.GameBus.Emit(GameEvt.PLAYER_INTERACTED, focusTrigger.ID);
                } else {
                    __.GameBus.Emit(GameEvt.PLAYER_INTERACTED_WITH_UNIT, focusTrigger.ID);
                }
            }
        }

        private void recalculation() {

            if (_gameplayStateRef.ControlPermissions[ControlPermission.ControlMovement]) {
                _input.Player.Enable();
                _uIRef.SetContextAction(UIContextAction.DefaultCrosshair);
            } else {
                _input.Player.Disable();
                _uIRef.SetContextAction(UIContextAction.None);
            }

            if (_gameplayStateRef.ControlPermissions[ControlPermission.ControlLook]) {
                _input.PlayerLook.Enable();
                _uIRef.SetContextAction(UIContextAction.DefaultCrosshair);
            } else {
                _input.PlayerLook.Disable();
                _uIRef.SetContextAction(UIContextAction.None);
            }

            if (_gameplayStateRef.ControlPermissions[ControlPermission.ControlGlobal]) {
                _input.Global.Enable();
                _uIRef.SetContextAction(UIContextAction.DefaultCrosshair);
            } else {
                _input.Global.Disable();
                _uIRef.SetContextAction(UIContextAction.None);
            }

            if (_playerRef.GameplayState.GameState == GameState.Paused) {

                _input.Global.Enable();
                _uIRef.SetContextAction(UIContextAction.DefaultCrosshair);

            } else if (_playerRef.GameplayState.GameState == GameState.InGame) {

                if (_gameplayStateRef.PlayerState == PlayerState.Interacting) {
                    if (_gameplayStateRef.InteractionType == InteractionType.WorldSelection) {

                        _input.Player.Fire.Enable();
                        _input.PlayerLook.Disable();

                        _captureMousePosition = true;
                    }
                } else if (_gameplayStateRef.PlayerState == PlayerState.Playing) {
                    //_input.Player.Fire.Disable();
                    _input.PlayerLook.Enable();
                    _captureMousePosition = false;
                }
            }
        }
    }
}
