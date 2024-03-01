using Game.Shared.Constants;
using System;

namespace Game.Shared.Interfaces {
    public interface IUI {
        UIContextAction UIContextAction { get; }
        Action<PlayerState> SetControlState { get; set; }
        void Init();
        void Open(InGameMenu inGameMenu, bool open = true);
        void SetContextAction(int? focusedId);
        void SetContextAction(UIContextAction uIContextAction = UIContextAction.DefaultCrosshair);
    }
}
