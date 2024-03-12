using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IInteractable {
        int ID { get; }
        public Action OnInteracted { get; set; }
        public Transform Transform { get; }

        void Init();
    }
}
