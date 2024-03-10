using Game.Shared.Bus;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Player {
    public class Player : MonoBehaviour, IPlayer {

        [SerializeField]
        private GameObject _unitGo;
        [SerializeField]
        private GameObject _uiGo;
        [SerializeField]
        private CameraController _cameraController;

        //-----------------------------------------------------------------------------------------------

        public IUnit Unit { get; private set; }
        public ICameraController CameraController { get => _cameraController; }
        public IGameplayState GameplayState { get; private set; }
        public IUI UI { get; private set; }
        public IPlayerControl PlayerControl { get; private set; }

        //-----------------------------------------------------------------------------------------------

        void Awake() {

            Unit = _unitGo.GetComponent<IUnit>();
            _unitGo = null;
            UI = _uiGo.GetComponent<IUI>();
            _uiGo = null;

            GameplayState = new GameplayState();
            GameplayState.SetState(
                GameState.InLoading,
                PlayerState.BrowsingMenu,
                UnitState.Hidden,
                InteractionType.None
            );

            GameplayState.OnGameplayRecalculation += gameplayRecalculation;

            __.GameBus.On(GameEvt.GAME_SCENE_LOADED, gameSceneLoaded);
        }

        void Start() {
            (Unit as IPlayerUnit).Init();

            _cameraController.Init(GameplayState, Unit);

            PlayerControl = GetComponent<IPlayerControl>();
            PlayerControl.Init(GameplayState, this);

            UI.Init(GameplayState);

            _cameraController.OnCameraFocussedInteractable += UI.SetContextAction;
        }

        void OnDestroy() {
            __.GameBus.UnregisterByEvent(GameEvt.GAME_SCENE_LOADED, gameSceneLoaded);
        }

        private void gameSceneLoaded(object obj) {
            var gameScene = obj as IGameScene;
            Debug.Log("__.GameBus.On -> GameEvt.GAME_SCENE_LOADED");
            gameScene.SetPlayer(this);
            gameScene.StartScene();
        }

        private void gameplayRecalculation() {

            Unit.UnitControl.Motor.SetUnitState(GameplayState.UnitState);
        }
    }
}
