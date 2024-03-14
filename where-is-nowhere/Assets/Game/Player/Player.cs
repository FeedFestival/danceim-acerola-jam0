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
        [SerializeField]
        private GameObject _ambientSoundManagerGo;

        private IAmbientSoundManager _ambientSoundManager;

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
            _ambientSoundManager = _ambientSoundManagerGo.GetComponent<IAmbientSoundManager>();
            _ambientSoundManagerGo = null;

            GameplayState = new GameplayState();

            GameplayState.SetState(
                GameState.InMainMenu,
                PlayerState.BrowsingMenu,
                UnitState.Hidden,
                InteractionType.None
            );

            __.GameBus.On(GameEvt.GAME_SCENE_LOADED, gameSceneLoaded);
            __.GameBus.On(GameEvt.PLAY_AMBIENT, onPlayAmbient);
        }

        void Start() {
            _ambientSoundManager.Init();
            (Unit as IPlayerUnit).Init(GameplayState);

            _cameraController.Init(GameplayState, Unit);

            PlayerControl = GetComponent<IPlayerControl>();
            PlayerControl.Init(GameplayState, this);

            UI.Init(GameplayState);

            GameplayState.ForceRecalculation();

            //_cameraController.OnCameraFocussedInteractable += UI.SetContextAction;
        }

        void OnDestroy() {
            __.GameBus.UnregisterByEvent(GameEvt.GAME_SCENE_LOADED, gameSceneLoaded);
            __.GameBus.UnregisterByEvent(GameEvt.PLAY_AMBIENT, onPlayAmbient);
        }

        private void gameSceneLoaded(object obj) {
            var gameScene = obj as IGameScene;
            Debug.Log("__.GameBus.On -> GameEvt.GAME_SCENE_LOADED");
            gameScene.SetPlayer(this);
            gameScene.StartScene();
        }

        private void onPlayAmbient(object obj) {
            _ambientSoundManager.PlayAmbient((AmbientSFXName)obj);
        }
    }
}
