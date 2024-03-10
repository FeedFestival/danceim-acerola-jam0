using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IPlayerControl {
        Action FirePerformed { get; set; }
        Action ExitInteraction { get; set; }

        void Init(IGameplayState gameplayState, IPlayer player);
        void ResetCrosshair();
    }
}