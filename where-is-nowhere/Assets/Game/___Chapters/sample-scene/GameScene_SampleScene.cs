using Game.Scene;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Chapters {
    public class GameScene_SampleScene : GameScene {

        public override void StartScene() {

            ftLightmaps.RefreshFull();
            Debug.Log("StartScene: -> ftLightmaps.RefreshFull()");


            PlayerInteracted += playerInteracted;

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

            (_interactableManager.Interactables[300007] as ISolvable).OnSolved += () => {
                (_interactableManager.Interactables[300008] as ILockable).Lock(false);
            };

            _interactableManager.Interactables[300006].OnInteracted += () => {
                Debug.Log("Interacted with simple door");
            };
        }
    }
}