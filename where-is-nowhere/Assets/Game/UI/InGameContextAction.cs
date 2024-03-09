using Game.Shared.Constants;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {
    public class InGameContextAction : MonoBehaviour {

        [SerializeField]
        private Image _defaultAction;
        [SerializeField]
        private Image _defaultCrosshair;
        [SerializeField]
        private Texture2D _crosshairTexture;

        public UIContextAction UIContextAction;

        internal void Init() {
            hideUIContextAction();
        }

        internal void SetContextAction(UIContextAction uIContextAction) {
            if (UIContextAction == uIContextAction) {
                return;
            }
            UIContextAction = uIContextAction;
            Debug.Log("UIContextAction: " + UIContextAction);

            switch (UIContextAction) {
                case UIContextAction.DefaultCrosshair:
                    _defaultCrosshair.gameObject.SetActive(true);
                    _defaultAction.gameObject.SetActive(false);
                    hideCursor();
                    break;
                case UIContextAction.MovingCrosshair:
                    _defaultCrosshair.gameObject.SetActive(true);
                    _defaultAction.gameObject.SetActive(false);
                    hideCursor(false);
                    Cursor.SetCursor(_crosshairTexture, Vector2.zero, CursorMode.ForceSoftware);
                    break;
                case UIContextAction.DefaultAction:
                    _defaultCrosshair.gameObject.SetActive(false);
                    _defaultAction.gameObject.SetActive(true);
                    hideCursor();
                    break;
                case UIContextAction.MenuAction:
                    hideUIContextAction(cursorHide: false);
                    break;
                case UIContextAction.None:
                default:
                    hideUIContextAction();
                    break;
            }
        }

        internal void SetMousePosition(Vector3 mousePosition) {
            if (UIContextAction == UIContextAction.MovingCrosshair) {
                _defaultCrosshair.rectTransform.anchoredPosition = mousePosition;
            }
        }

        private void hideUIContextAction(bool cursorHide = true) {
            _defaultCrosshair.gameObject.SetActive(false);
            _defaultAction.gameObject.SetActive(false);
            hideCursor(cursorHide);
        }

        private void hideCursor(bool hide = true) {
            Cursor.lockState = !hide ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = !hide;
        }
    }
}
