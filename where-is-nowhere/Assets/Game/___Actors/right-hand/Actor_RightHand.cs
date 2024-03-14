using Game.Shared.Components;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using Game.Shared.Services;
using UnityEngine;

namespace Game.Actors {
    public class Actor_RightHand : Actor {

        [Header("Actor Right Hand")]
        [SerializeField]
        private GameObject _focusTriggerGo;
        private IFocusTrigger _focusTrigger;

        [Header("Highlight Settings")]
        [SerializeField]
        private Renderer _renderer;
        [SerializeField]
        private Material _highlightMaterial;
        private int _highlightIndex;

        public override void Init(int id) {
            base.Init(id);
            initFocusTrigger(id);
        }

        void OnDestroy() {
            if (_focusTrigger != null) {
                _focusTrigger.OnFocussed -= onFocused;
            }
        }

        private void onFocused(bool isFocused) {
            //if (isFocused) {
            //    TriggerService.ChangeSharedMaterial(ref _renderer, ref _highlightIndex, _highlightMaterial);
            //} else {
            //    TriggerService.ChangeSharedMaterial(ref _renderer, ref _highlightIndex);
            //}
        }

        private void initFocusTrigger(int id) {
            if (_focusTriggerGo != null) {
                _focusTrigger = _focusTriggerGo.GetComponent<IFocusTrigger>();
                if (_focusTrigger != null) {
                    _focusTrigger.Init(id, InteractType.Unit);
                    _focusTrigger.OnFocussed += onFocused;

                    //TriggerService.SetupSharedMaterialForHighlight(ref _renderer, ref _highlightIndex);
                }
            }
        }
    }
}
