using Game.Shared.Constants;
using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUI {
        UIContextAction UIContextAction { get; }
        void Init(IGameplayState gameplayState);
        void Open(InGameMenu inGameMenu, bool open = true);
        void SetContextAction(int? focusedId, string additionalText = "");
        void SetContextAction(UIContextAction uIContextAction = UIContextAction.DefaultCrosshair);
        void SetMousePosition(Vector3 mousePosition);
    }
}
