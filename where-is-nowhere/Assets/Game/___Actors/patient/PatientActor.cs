using Game.Shared.Components;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Actors {
    public class PatientActor : Actor, IPatientActor {

        [SerializeField]
        private DetashedHands _detashedHands;

        public void Init(IInventory inventory) {

            _detashedHands.ShowRightHand(false);

            inventory.InventoryItemAdded += inventoryItemAdded;
            inventory.InventoryItemRemoved += inventoryItemRemoved;
        }

        private void inventoryItemRemoved(InventoryItem inventoryItem) {

            if (inventoryItem == InventoryItem.RightHand) {
                _detashedHands.ShowRightHand(false);
            }
        }

        private void inventoryItemAdded(InventoryItem inventoryItem) {
            if (inventoryItem == InventoryItem.RightHand) {
                _detashedHands.ShowRightHand();
            }
        }
    }
}