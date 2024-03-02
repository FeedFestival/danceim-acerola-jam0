using Game.Shared.Interfaces;
using Game.Shared.Services;
using System;
using UnityEngine;

namespace Game.Interactable {
    public class BaseInteractable : MonoBehaviour, IInteractable {

        [SerializeField]
        private GameObject _focusTriggerGo;
        private IFocusTrigger _focusTrigger;
        [Header("Highlight Settings")]
        [SerializeField]
        private Renderer _renderer;
        [SerializeField]
        private Material _highlightMaterial;
        private int _highlightIndex;

        //-----------------------------------------------------------------------------------

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
                _focusTrigger.OnFocussed += onFocused;

                TriggerService.SetupSharedMaterialForHighlight(ref _renderer, ref _highlightIndex);
            }
        }

        public virtual void DoDefaultInteraction(IPlayer player) {
            OnInteracted?.Invoke();
        }

        public virtual void DoDefaultInteraction(IUnit unit) {
            OnInteracted?.Invoke();
        }

        //-----------------------------------------------------------------------------------

        void OnDestroy() {
            if (_focusTrigger != null) {
                _focusTrigger.OnFocussed -= onFocused;
            }
        }

        //-----------------------------------------------------------------------------------

        protected virtual void onFocused(bool isFocused) {
            if (isFocused) {
                TriggerService.ChangeSharedMaterial(ref _renderer, ref _highlightIndex, _highlightMaterial);
            } else {
                TriggerService.ChangeSharedMaterial(ref _renderer, ref _highlightIndex);
            }
        }
    }
}
