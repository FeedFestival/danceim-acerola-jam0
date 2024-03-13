using Game.Interactable;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables {
    public class InteractableItem : BaseInteractable, IRequiredItems {
        public InventoryItem[] RequiredItems { get; set; }

        public override void Init() {
            base.initEntityId();
            base.initFocusTrigger();
        }

        public override void DoDefaultInteraction(IPlayer player) {
            OnInteracted?.Invoke();

            _focusTrigger.Enable(false);

            gameObject.SetActive(false);
        }
    }
}