using Game.Shared.Bus;
using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.Scene {
    public class GameScene : MonoBehaviour, IGameScene {
        [SerializeField]
        private GameObject _unitManagerGo;
        [SerializeField]
        private GameObject _interactableManagerGo;
        protected IUnitManager _unitManager;
        protected IInteractableManager _interactableManager;
        protected IPlayer _player;

        public Action<int> PlayerInteracted;

        private void Awake() {
            if (_unitManagerGo) {
                _unitManager = _unitManagerGo?.GetComponent<IUnitManager>();
            }
            if (_interactableManagerGo) {
                _interactableManager = _interactableManagerGo?.GetComponent<IInteractableManager>();
            }
        }

        void OnDestroy() {
            __.GameBus.UnregisterByEvent(GameEvt.PLAYER_INTERACTED, onPlayerInteracted);
        }

        void OnApplicationQuit() {
            Debug.Log("ApplicationQuit - Reseting Static Properties");
            __.ClearAll();
        }

        protected virtual void Start() {
            Debug.Log("GameScene  -> Start -> GameEvt.GAME_SCENE_LOADED");

            _unitManager.Init();
            _interactableManager?.Init();

            __.GameBus.Emit(GameEvt.GAME_SCENE_LOADED, this);

            __.GameBus.On(GameEvt.PLAYER_INTERACTED, onPlayerInteracted);
        }

        public void SetPlayer(IPlayer player) {
            _player = player;
        }

        public virtual void StartScene() { }
        protected virtual void onPlayerInteracted(object obj) {
            PlayerInteracted?.Invoke((int)obj);
        }
    }
}