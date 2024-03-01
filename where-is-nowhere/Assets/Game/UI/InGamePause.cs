using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI {
    public class InGamePause : MonoBehaviour {

        [SerializeField]
        private Button _resumeButton;

        [SerializeField]
        private Button _optionsButton;

        [SerializeField]
        private Button _quitButton;

        private Action _onScreenDismissed;

        internal void Init(Action onScreenDismissed) {

            _onScreenDismissed = onScreenDismissed;

            _resumeButton?.onClick.AddListener(() => {
                _onScreenDismissed?.Invoke();
                gameObject.SetActive(false);
            });

            _optionsButton?.onClick.AddListener(() => {
                Debug.Log("Go To Options -> ");
            });

            _quitButton?.onClick.AddListener(() => SceneManager.LoadSceneAsync("end"));
        }
    }
}