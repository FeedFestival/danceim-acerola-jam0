using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.Interactable {
    public class BaseInteractable : MonoBehaviour, IInteractable {

        [SerializeField]
        private GameObject _focusTriggerGo;
        private IFocusTrigger _focusTrigger;

        public int ID { get; private set; }
        public Action OnInteracted { get; private set; }
        public Transform Transform { get => transform; }
        public virtual void Init() {
            var entity = gameObject.GetComponent<IEntityId>();
            ID = entity.Id;
            entity.DestroyComponent();

            _focusTrigger = _focusTriggerGo?.GetComponent<IFocusTrigger>();
            if (_focusTrigger != null) {
                _focusTrigger.Init(ID);
            }
        }

        //-----------------------------------------------------------------------------------

        public virtual void DoDefaultInteraction(IPlayer player) {
            OnInteracted?.Invoke();
        }

        public virtual void DoDefaultInteraction(IUnit unit) {
            OnInteracted?.Invoke();
        }
    }
}
