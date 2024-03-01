using Game.Scene;
using Game.Shared.Constants;
using UnityEngine;

namespace Game.Chapters {
    public class GameScene_SampleScene : GameScene {

        public override void StartScene() {

            Debug.Log("StartScene: ");

            PlayerInteracted += playerInteracted;

            _player.SetGameState(GameState.InGame);
            _player.SetControlState(PlayerState.Playing);
        }

        private void playerInteracted(int entityId) {
            Debug.Log("playerInteracted.entityId: " + entityId);
            _interactableManager.Interactables[entityId].DoDefaultInteraction(_player);
        }
    }
}