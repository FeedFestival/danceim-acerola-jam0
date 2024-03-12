using Game.Shared.Constants;
using System;

namespace Game.Shared.Interfaces {
    public interface IFocusTrigger {
        int ID { get; }
        Action<bool> OnFocussed { get; set; }
        InteractType Type { get; }

        void Init(int id, InteractType interactType = InteractType.Interactable);
        void SetIsFocused(bool isFocused = true);
        void Enable(bool enable = true);
    }
}