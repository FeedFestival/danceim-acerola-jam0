using System;

namespace Game.Shared.Interfaces {
    public interface IMinigameFocusTrigger {
        string Key { get; }
        Action<string> OnFocussed { get; set; }
        void Init(string key);
        void SetIsFocused(bool isFocused = true);
        void Enable(bool enable = true);
    }
}