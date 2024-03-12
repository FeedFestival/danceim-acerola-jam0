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
        [SerializeField]
        private GameObject _zoneManagerGo;
        protected IUnitManager _unitManager;
        protected IInteractableManager _interactableManager;
        protected IZoneManager _zoneManager;
        protected IPlayer _player;

        public Action<int> PlayerInteracted;
        public Action<int> PlayerInteractedUnit;

        private void Awake() {
            if (_unitManagerGo != null) {
                _unitManager = _unitManagerGo?.GetComponent<IUnitManager>();
            }
            if (_interactableManagerGo != null) {
                _interactableManager = _interactableManagerGo?.GetComponent<IInteractableManager>();
            }
            if (_zoneManagerGo != null) {
                _zoneManager = _zoneManagerGo?.GetComponent<IZoneManager>();
            }
        }

        void OnDestroy() {
            __.GameBus.UnregisterByEvent(GameEvt.PLAYER_INTERACTED, onPlayerInteracted);
            __.GameBus.UnregisterByEvent(GameEvt.PLAYER_INTERACTED, onPlayerInteractedUnit);
        }

        void OnApplicationQuit() {
            Debug.Log("ApplicationQuit - Reseting Static Properties");
            __.ClearAll();
        }

        protected virtual void Start() {
            Debug.Log("GameScene  -> Start -> GameEvt.GAME_SCENE_LOADED");

            _unitManager?.Init();
            _interactableManager?.Init();
            _zoneManager?.Init();

            __.GameBus.Emit(GameEvt.GAME_SCENE_LOADED, this);

            __.GameBus.On(GameEvt.PLAYER_INTERACTED, onPlayerInteracted);
            __.GameBus.On(GameEvt.PLAYER_INTERACTED_WITH_UNIT, onPlayerInteractedUnit);
        }

        public void SetPlayer(IPlayer player) {
            _player = player;
        }

        public virtual void StartScene() { }
        protected virtual void onPlayerInteracted(object obj) {
            PlayerInteracted?.Invoke((int)obj);
        }
        protected virtual void onPlayerInteractedUnit(object obj) {
            PlayerInteractedUnit?.Invoke((int)obj);
        }
    }
}