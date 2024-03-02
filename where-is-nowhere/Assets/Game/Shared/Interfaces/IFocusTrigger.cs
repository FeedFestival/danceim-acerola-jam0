using System;

namespace Game.Shared.Interfaces {
    public interface IFocusTrigger {
        int ID { get; }
        Action<bool> OnFocussed { get; set; }
        void Init(int id);
        void SetIsFocused(bool isFocused = true);
    }
}