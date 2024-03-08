using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.Shared.Core {
    public class FocusTrigger : MonoBehaviour, IFocusTrigger {
        public int ID { get; private set; }
        public Action<bool> OnFocussed { get; set; }
        public void Init(int id) {
            ID = id;
        }
        public void SetIsFocused(bool isFocused = true) {
            OnFocussed?.Invoke(isFocused);
        }

        public void Enable(bool enable = true) {
            GetComponent<BoxCollider>().enabled = enable;
        }
    }
}