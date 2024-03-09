using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.Interactables {
    public class WallKeypadKey : MonoBehaviour, IMinigameFocusTrigger {
        private BoxCollider _boxCollider;

        //---------------------------------------------------------------------------------------------

        public string Key { get; private set; }
        public Action<string> OnFocussed { get; set; }

        public void Init(string key) {
            Key = key;

            _boxCollider = GetComponent<BoxCollider>();
        }

        public void SetIsFocused(bool isFocused = true) {
            OnFocussed?.Invoke(Key);
        }
        public void Enable(bool enable = true) {
            _boxCollider.enabled = enabled;
        }

        //---------------------------------------------------------------------------------------------
    }
}