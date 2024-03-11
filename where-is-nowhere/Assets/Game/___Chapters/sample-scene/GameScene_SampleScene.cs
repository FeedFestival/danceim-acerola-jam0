using Game.Scene;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Chapters {
    public class GameScene_SampleScene : GameScene {

        private const string _sceneName = "awakening";
        private const string _nextSceneName = "end";
        private IEnumerator _nextScene;

        [SerializeField]
        private Transform _startPoint;

        public override void StartScene() {

            ftLightmaps.RefreshFull();
            LightProbes.Tetrahedralize();

            PlayerInteracted += playerInteracted;

            _player.Unit.Transform.position = _startPoint.position;
            (_player.Unit.UnitControl as IPlayerUnitControl).Teleport(_startPoint.position);

            _player.GameplayState.SetState(
                GameState.InGame,
                PlayerState.Playing,
                UnitState.FreePlaying,
                InteractionType.None
            );

            foreach (var unit in _unitManager.Units) {
                (unit.Value.UnitControl as INPCControl).Motor.DestinationReached += onDestinationReached;
                findNewPositionToGo(unit.Value);
            }

            setupInteractions();
        }

        private void playerInteracted(int entityId) {
            Debug.Log("playerInteracted.entityId: " + entityId);
            _interactableManager.Interactables[entityId].DoDefaultInteraction(_player);
        }

        private void onDestinationReached(int id) {
            findNewPositionToGo(_unitManager.Units[id]);
        }

        private void findNewPositionToGo(IUnit unit) {
            var randomPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
            (unit.UnitControl as INPCControl).MoveTo(randomPos);
        }

        private void setupInteractions() {

            _zoneManager.Zones[100001].Entered += goNextScene;

            (_interactableManager.Interactables[300007] as ISolvable).OnSolved += () => {
                (_interactableManager.Interactables[300008] as ILockable).Lock(false);
            };

            _interactableManager.Interactables[300006].OnInteracted += () => {
                Debug.Log("Interacted with simple door");
            };
        }

        private void goNextScene() {

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