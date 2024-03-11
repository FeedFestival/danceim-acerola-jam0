using System;

namespace Game.Shared.Interfaces {
    public interface IMainMenuController {
        Action StartGame { get; set; }

        void Init();
        void Show(bool show = true);
    }
}