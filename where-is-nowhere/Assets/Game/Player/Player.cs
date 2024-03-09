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
        private GameState _gameState;
        [SerializeField]
        private PlayerState _playerState;
        [SerializeField]
        private InteractionType _interactionType;

        //-----------------------------------------------------------------------------------------------

        public IUnit Unit { get; private set; }
        public ICameraController CameraController { get => _cameraController; }
        public GameState GameState { get => _gameState; }
        public PlayerState PlayerState { get => _playerState; }
        public IUI UI { get; private set; }
        public IPlayerControl PlayerControl { get; private set; }
        public void SetGameState(GameState gameState) {
            if (_gameState == gameState) {
                return;
            }
            _gameState = gameState;
            Debug.Log("_gameState: " + _gameState);

            switch (_gameState) {
                case GameState.InMainMenu:

                    Unit.SetUnitState(UnitState.Hidden);
                    _cameraController.SetCameraControl(CameraControl.Look, false);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlLook, false);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlMovement, false);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlGlobal, false);

                    break;
                case GameState.InLoading:
                    break;
                case GameState.InGame:
                default:

                    Unit.SetUnitState(UnitState.FreePlaying);
                    _cameraController.SetCameraControl(CameraControl.Look, true);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlLook, true);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlMovement, true);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlGlobal, true);

                    break;
            }
        }
        public void SetControlState(PlayerState playerState) {
            if (_playerState == playerState) {
                return;
            }
            _playerState = playerState;
            Debug.Log("_playerState: " + _playerState);

            switch (_playerState) {
                case PlayerState.BrowsingMenu:

                    Unit.SetUnitState(UnitState.Interacting);
                    _cameraController.SetCameraControl(CameraControl.Look, false);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlLook, false);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlMovement, false);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlGlobal, true);

                    break;
                case PlayerState.WatchingCutcene:

                    break;
                case PlayerState.Interacting:

                    Unit.SetUnitState(UnitState.Interacting);

                    _cameraController.SetCameraControl(CameraControl.Look, false);

                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlLook, false);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlMovement, false);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlGlobal, true);

                    break;
                case PlayerState.Playing:
                default:

                    Unit.SetUnitState(UnitState.FreePlaying);
                    _cameraController.SetCameraControl(CameraControl.Look, true);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlLook, true);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlMovement, true);
                    (PlayerControl as PlayerControl).EnableControlPermission(ControlPermission.ControlGlobal, true);

                    break;
            }
        }
        public void SetInteractionControl(InteractionType interactionType) => (PlayerControl as PlayerControl).SetInteractionControl(interactionType);
        //-----------------------------------------------------------------------------------------------

        void Awake() {

            Unit = _unitGo.GetComponent<IUnit>();
            _unitGo = null;
            UI = _uiGo.GetComponent<IUI>();
            _uiGo = null;

            __.GameBus.On(GameEvt.GAME_SCENE_LOADED, gameSceneLoaded);
        }

        void Start() {
            (Unit as IPlayerUnit).Init();

            _cameraController.Init(Unit);

            PlayerControl = GetComponent<IPlayerControl>();
            (PlayerControl as PlayerControl).Init(this);

            UI.Init();
            UI.SetControlState += SetControlState;

            _cameraController.OnCameraFocussedInteractable += UI.SetContextAction;

            SetControlState(PlayerState.BrowsingMenu);
        }

        void OnDestroy() {
            UI.SetControlState -= SetControlState;
            __.GameBus.UnregisterByEvent(GameEvt.GAME_SCENE_LOADED, gameSceneLoaded);
        }

        private void gameSceneLoaded(object obj) {
            var gameScene = obj as IGameScene;
            Debug.Log("__.GameBus.On -> GameEvt.GAME_SCENE_LOADED");
            gameScene.SetPlayer(this);
            gameScene.StartScene();
        }
    }
}
