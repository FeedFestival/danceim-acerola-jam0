using Game.Shared.Interfaces;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {
    public class MainMenuController : MonoBehaviour, IMainMenuController {

        public Action StartGame { get; set; }

        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Button _optionsButton;

        [SerializeField]
        private Button _quitButton;

        public void Init() {
            _startButton.onClick.AddListener(() => StartGame?.Invoke());

            _optionsButton.onClick.AddListener(() => {
                Debug.Log("Go To Options -> ");
            });

            _quitButton.onClick.AddListener(() => {
                Application.Quit();
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#endif
            });
        }

        public void Show(bool show = true) {
            gameObject.SetActive(show);
        }
    }
}