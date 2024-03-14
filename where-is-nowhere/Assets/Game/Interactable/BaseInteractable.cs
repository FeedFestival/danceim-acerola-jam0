using Game.Shared.Interfaces;
using Game.Shared.Services;
using System;
using UnityEngine;

namespace Game.Interactable {
    public class BaseInteractable : MonoBehaviour, IInteractable, IDefaultInteraction {

        [SerializeField]
        private GameObject _focusTriggerGo;
        protected IFocusTrigger _focusTrigger;
        [Header("Highlight Settings")]
        [SerializeField]
        private Renderer _renderer;
        [SerializeField]
        private Material _highlightMaterial;
        private int _highlightIndex;

        //-----------------------------------------------------------------------------------

        public int ID { get; private set; }
        public Action OnInteracted { get; set; }
        public Transform Transform { get => transform; }
        public virtual void Init() {
            initEntityId();
            initFocusTrigger();

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
            //if (isFocused) {
            //    TriggerService.ChangeSharedMaterial(ref _renderer, ref _highlightIndex, _highlightMaterial);
            //} else {
            //    TriggerService.ChangeSharedMaterial(ref _renderer, ref _highlightIndex);
            //}
        }

        protected void initEntityId() {
            var entity = gameObject.GetComponent<IEntityId>();
            ID = entity.Id;
            entity.DestroyComponent();
        }

        protected void initFocusTrigger() {
            if (_focusTriggerGo != null) {
                _focusTrigger = _focusTriggerGo.GetComponent<IFocusTrigger>();
                if (_focusTrigger != null) {
                    _focusTrigger.Init(ID);
                    _focusTrigger.OnFocussed += onFocused;

                    //TriggerService.SetupSharedMaterialForHighlight(ref _renderer, ref _highlightIndex);
                }
            }
        }
    }
}
