using System.Collections.Generic;

namespace Game.Shared.Interfaces {
    public interface IInteractableManager {
        public Dictionary<int, IInteractable> Interactables { get; set; }
        void Init();
    }
}
