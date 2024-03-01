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

        public UIContextAction UIContextAction { get => _inGameContextAction.UIContextAction; }
        public Action<PlayerState> SetControlState { get; set; }

        public void Init() {

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

        public void SetContextAction(int? focusedId) {
            var uIContextAction = UIContextAction.DefaultCrosshair;
            if (focusedId.HasValue) {
                uIContextAction = UIContextAction.DefaultAction;
            }

            SetContextAction(uIContextAction);
        }

        public void SetContextAction(UIContextAction uIContextAction = UIContextAction.DefaultCrosshair) {
            _inGameContextAction.SetContextAction(uIContextAction);
        }

        //----------------------------------------------------------------------------------------------

        private void onScreenDismissed() {
            SetControlState?.Invoke(PlayerState.Playing);
        }
    }
}
