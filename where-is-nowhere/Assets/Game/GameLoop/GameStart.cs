using Game.Shared.Bus;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;
using Cinemachine;

namespace Game.Loop {
    public class GameStart : MonoBehaviour {

        public Action OnDependenciesLoaded;

        [SerializeField]
        private GameObject _mainMenuControllerGo;
        private IMainMenuController _mainMenuController;

        private SceneDependency[] _scenesToLoad;
        private int _loadedIndex = -1;
        private Action<string> _onSceneLoaded;
        private IEnumerator _loadScene;
        private IEnumerator _starGameScene;

        [SerializeField]
        private Transform _startPoint;

        [SerializeField]
        private PlayableDirector _startPlayableDirector;
        [SerializeField]
        private GameObject _virtualCamera;

        private struct SceneDependency {
            public string name;
            public Action action;
        }

        private void Start() {

            ftLightmaps.RefreshFull();
            LightProbes.Tetrahedralize();

            _mainMenuController = _mainMenuControllerGo.GetComponent<IMainMenuController>();
            _mainMenuControllerGo = null;
            _mainMenuController.Show(false);

            _virtualCamera.SetActive(false);

            StartCoroutine(delayedStart());
        }

        private void OnDestroy() {
            _mainMenuController.StartGame -= initStartGame;
        }

        private IEnumerator delayedStart() {
            yield return new WaitForSeconds(0.3f);

            var playerGo = GameObject.Find("PLAYER");
            if (playerGo != null) {
                StartCoroutine(playerCameFromEndScene(playerGo));
            } else {

                _scenesToLoad = new SceneDependency[1] {
                    new SceneDependency() {
                        name = "Player",
                        action = onPlayerLoaded
                    }
                };

                OnDependenciesLoaded += onDependenciesLoaded;
                _onSceneLoaded += onSceneLoaded;

                loadNext();
            }
        }

        private IEnumerator playerCameFromEndScene(GameObject playerGo) {

            yield return new WaitForSeconds(0.33f);

            var player = playerGo.GetComponent<IPlayer>();
            if (player != null) {

                player.Unit.Transform.position = _startPoint.position;
                player.Unit.UnitControl.Teleport(_startPoint.position);

                yield return new WaitForSeconds(0.33f);

                player.GameplayState.SetState(
                    GameState.InGame,
                    PlayerState.Playing,
                    UnitState.FreePlaying,
                    InteractionType.None
                );
            }
        }

        private void onDependenciesLoaded() {

            _onSceneLoaded -= onSceneLoaded;
            OnDependenciesLoaded -= onDependenciesLoaded;

            StopCoroutine(_loadScene);

            _mainMenuController.Show();
            _mainMenuController.Init();
            _mainMenuController.StartGame += initStartGame;

            // TODO: move player to see the crack in the wall
        }

        private void loadNext() {

            if (_loadedIndex == _scenesToLoad.Length - 1) {
                OnDependenciesLoaded?.Invoke();
                return;
            }

            _loadedIndex++;
            _loadScene = loadScene(_scenesToLoad[_loadedIndex].name);
            StartCoroutine(_loadScene);
        }

        private void onSceneLoaded(string sceneName) {
            Debug.Log((_loadedIndex + 1) + ". " + sceneName + " has been loaded.");

            _scenesToLoad[_loadedIndex].action?.Invoke();

            loadNext();
        }

        private IEnumerator loadScene(string sceneName) {
            var asyncLoadLevel = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncLoadLevel.isDone) {
                yield return null;
            }

            _onSceneLoaded?.Invoke(sceneName);
        }

        private void initStartGame() {
            Debug.Log("StartCoroutine(_starGameScene); -> ");
            _starGameScene = startGameScene();
            StartCoroutine(_starGameScene);
        }

        private IEnumerator startGameScene() {

            //_player.GameplayState.SetState(
            //    GameState.InLoading,
            //    PlayerState.BrowsingMenu,
            //    UnitState.Hidden,
            //    InteractionType.None
            //);

            yield return new WaitForSeconds(0.1f);

            var loadSceneParameters = new LoadSceneParameters();
            loadSceneParameters.loadSceneMode = LoadSceneMode.Additive;

            // if you wan't to switch to a 2D physics, othersiew don't add
            // loadSceneParameters.localPhysicsMode = LocalPhysicsMode.Physics3D;

            var scene = SceneManager.LoadScene(1, loadSceneParameters);

            while (!scene.isLoaded) {
                yield return null;
            }

            // Ensure that the newly loaded scene is set as the active scene
            var loadedScene = SceneManager.GetSceneByBuildIndex(1);
            if (loadedScene.IsValid() && loadedScene.isLoaded) {
                SceneManager.SetActiveScene(loadedScene);
            } else {
                Debug.LogError("Failed to set the active scene.");
            }

            SceneManager.UnloadSceneAsync(0);

            StopCoroutine(_starGameScene);
        }

        private void onPlayerLoaded() {
            _scenesToLoad[_loadedIndex].action -= onPlayerLoaded;

            StartCoroutine(waitAndPlayAmbient());

            Debug.Log("Do something on Player loaded");
        }

        private IEnumerator waitAndPlayAmbient() {
            yield return new WaitForSeconds(0.1f);

            __.GameBus.Emit(GameEvt.PLAY_AMBIENT, AmbientSFXName.InsideWalls);

            var playerGo = GameObject.Find("PLAYER");
            var player = playerGo.GetComponent<IPlayer>();
            if (player != null) {

                var timeline = _startPlayableDirector.playableAsset as TimelineAsset;
                if (timeline != null) {

                    _virtualCamera.SetActive(true);

                    foreach (var track in timeline.GetOutputTracks()) {
                        if (track is CinemachineTrack) {
                            var brain = player.CameraController.Camera.GetComponent<CinemachineBrain>();
                            _startPlayableDirector
                                .SetGenericBinding(track, brain);

                            _startPlayableDirector.Play();
                        }
                    }
                }
            }
        }
    }
}