using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IInteractable {
        int ID { get; }
        public Action OnInteracted { get; }
        public Transform Transform { get; }

        void Init();
        void DoDefaultInteraction(IPlayer player);
        void DoDefaultInteraction(IUnit unit);
    }
}
