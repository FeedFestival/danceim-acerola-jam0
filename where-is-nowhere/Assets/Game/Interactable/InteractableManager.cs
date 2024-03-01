using Game.Shared.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactable {
    public class InteractableManager : MonoBehaviour, IInteractableManager {

        public Dictionary<int, IInteractable> Interactables { get; set; }

        public void Init() {

            Interactables = new Dictionary<int, IInteractable>();
            foreach (Transform ct in transform) {
                if (ct.gameObject.activeSelf == false) { continue; }

                var interactable = ct.GetComponent<IInteractable>();
                interactable?.Init();
                Interactables.Add(interactable.ID, interactable);
            }
        }

        //-----------------------------------------------------------------------------------------
    }
}
