using Game.Shared.Constants;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {
    public class InGameContextAction : MonoBehaviour {

        [SerializeField]
        private Image _defaultAction;
        [SerializeField]
        private Image _defaultCrosshair;

        public UIContextAction UIContextAction;

        internal void Init() {
            hideUIContextAction();
        }

        internal void SetContextAction(UIContextAction uIContextAction) {
            if (UIContextAction == uIContextAction) {
                return;
            }
            UIContextAction = uIContextAction;

            switch (UIContextAction) {
                case UIContextAction.DefaultCrosshair:
                    _defaultCrosshair.gameObject.SetActive(true);
                    _defaultAction.gameObject.SetActive(false);
                    hideCursor();
                    break;
                case UIContextAction.DefaultAction:
                    _defaultCrosshair.gameObject.SetActive(false);
                    _defaultAction.gameObject.SetActive(true);
                    hideCursor();
                    break;
                case UIContextAction.MenuAction:
                    hideUIContextAction();
                    break;
                case UIContextAction.None:
                default:
                    hideUIContextAction();
                    break;
            }
        }

        private void hideUIContextAction() {
            _defaultCrosshair.gameObject.SetActive(false);
            _defaultAction.gameObject.SetActive(false);
            hideCursor(false);
        }

        private void hideCursor(bool hide = true) {
            Cursor.lockState = !hide ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = !hide;
        }
    }
}
