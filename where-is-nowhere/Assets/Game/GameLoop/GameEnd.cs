using Game.Scene;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Loop {
    public class GameEnd : GameScene {

        [Header("GameEnd")]
        [SerializeField]
        private Transform _startPoint;
        private const string _sceneName = "end";
        private const string _nextSceneName = "start";
        private IEnumerator _nextScene;

        public override void StartScene() {

            Debug.Log("GameEnd -> : StartScene");

            ftLightmaps.RefreshFull();
            LightProbes.Tetrahedralize();

            _zoneManager.Zones[100002].Entered += goNextScene;

            StartCoroutine(movePlayer());
        }

        //---------------------------------------------------------------------------------------

        private void OnDestroy() {
            _zoneManager.Zones[100002].Entered -= goNextScene;
        }

        //---------------------------------------------------------------------------------------

        private IEnumerator movePlayer() {

            yield return new WaitForSeconds(0.1f);

            _player.Unit.Transform.position = _startPoint.position;
            (_player.Unit.UnitControl as IPlayerUnitControl).Teleport(_startPoint.position);

            _player.GameplayState.SetState(
                GameState.InGame,
                PlayerState.Playing,
                UnitState.FreePlaying,
                InteractionType.None
            );
        }

        private void goNextScene() {

            Debug.Log("goEndGameScene(): ");

            _nextScene = goEndGameScene();
            StartCoroutine(_nextScene);
        }

        private IEnumerator goEndGameScene() {

            _player.GameplayState.SetState(
                GameState.InLoading,
                PlayerState.BrowsingMenu,
                UnitState.Hidden,
                InteractionType.None
            );

            yield return new WaitForSeconds(0.1f);

            var loadSceneParameters = new LoadSceneParameters();
            loadSceneParameters.loadSceneMode = LoadSceneMode.Additive;

            var scene = SceneManager.LoadScene(_nextSceneName, loadSceneParameters);

            while (!scene.isLoaded) {
                yield return null;
            }

            // Ensure that the newly loaded scene is set as the active scene
            var loadedScene = SceneManager.GetSceneByName(_nextSceneName);
            //var loadedScene = SceneManager.GetSceneByBuildIndex(1);
            if (loadedScene.IsValid() && loadedScene.isLoaded) {
                SceneManager.SetActiveScene(loadedScene);
            } else {
                Debug.LogError("Failed to set the active scene.");
            }

            SceneManager.UnloadSceneAsync(_sceneName);

            StopCoroutine(_nextScene);
        }
    }
}