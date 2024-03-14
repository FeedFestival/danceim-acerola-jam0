using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.UI {
    public class UI : MonoBehaviour, IUI {
        [SerializeField]
        private InGamePause _inGamePause;
        [SerializeField]
        private InGameContextAction _inGameContextAction;
        private InGameMenu _inGameMenu;
        private IGameplayState _gameplayStateRef;

        public UIContextAction UIContextAction { get => _inGameContextAction.UIContextAction; }

        public void Init(IGameplayState gameplayState) {
            _gameplayStateRef = gameplayState;
            _gameplayStateRef.OnGameplayRecalculation += recalculate;

            _inGamePause.Init(onScreenDismissed);

            _inGameContextAction.Init();

            Open(InGameMenu.Pause, false);
        }

        public void Open(InGameMenu inGameMenu, bool open = true) {
            if (_inGameMenu == inGameMenu) {
                return;
            }
            _inGameMenu = inGameMenu;

            switch (_inGameMenu) {
                case InGameMenu.None:
                    break;
                case InGameMenu.Pause:
                    _inGamePause.gameObject.SetActive(open);
                    break;
                default:
                    break;
            }
        }

        public void SetContextAction(int? focusedId, string additionalText = "") {

            var uIContextAction = UIContextAction.DefaultCrosshair;
            if (focusedId.HasValue) {
                uIContextAction = UIContextAction.DefaultAction;
            }

            _inGameContextAction.SetAdditionalText(additionalText);
            SetContextAction(uIContextAction);
        }

        public void SetContextAction(UIContextAction uIContextAction = UIContextAction.DefaultCrosshair) {
            _inGameContextAction.SetContextAction(uIContextAction);
        }

        public void SetMousePosition(Vector3 mousePosition) {
            _inGameContextAction.SetMousePosition(mousePosition);
        }

        //----------------------------------------------------------------------------------------------

        private void onScreenDismissed() {
            _gameplayStateRef.SetState(
                GameState.InGame,
                PlayerState.Playing
            );
        }

        private void recalculate() {

            if (_gameplayStateRef.GameState == GameState.InMainMenu) {

                _inGameContextAction.SetContextAction(UIContextAction.MenuAction);

            } else if (_gameplayStateRef.GameState == GameState.InGame) {

                if (_gameplayStateRef.ControlPermissions[ControlPermission.ControlMovement]) {
                    SetContextAction(UIContextAction.DefaultCrosshair);
                } else {
                    SetContextAction(UIContextAction.None);
                }

                if (_gameplayStateRef.ControlPermissions[ControlPermission.ControlLook]) {
                    SetContextAction(UIContextAction.DefaultCrosshair);
                } else {
                    SetContextAction(UIContextAction.None);
                }

                if (_gameplayStateRef.ControlPermissions[ControlPermission.ControlGlobal]) {
                    SetContextAction(UIContextAction.DefaultCrosshair);
                } else {
                    SetContextAction(UIContextAction.None);
                }

                if (_gameplayStateRef.PlayerState == PlayerState.Interacting) {
                    if (_gameplayStateRef.InteractionType == InteractionType.WorldSelection) {
                        SetContextAction(UIContextAction.MovingCrosshair);
                    }
                } else {

                }
            }
        }
    }
}
