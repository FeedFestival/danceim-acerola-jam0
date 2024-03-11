using Game.Shared.Constants;
using System;
using System.Collections.Generic;

namespace Game.Shared.Interfaces {
    public interface IGameplayState {
        GameState GameState { get; }
        PlayerState PlayerState { get; }
        UnitState UnitState { get; }
        InteractionType InteractionType { get; }
        Action OnGameplayRecalculation { get; set; }
        Dictionary<CameraControl, bool> CameraControlPermissions { get; }
        Dictionary<ControlPermission, bool> ControlPermissions { get; }

        void SetState(
            GameState gameState,
            PlayerState playerState,
            bool emit = true
        );

        void SetState(
            GameState gameState,
            PlayerState playerState,
            UnitState unitState,
            bool emit = true
        );

        void SetState(
            GameState gameState,
            PlayerState playerState,
            UnitState unitState,
            InteractionType interactionType,
            bool emit = true
        );
        void ForceRecalculation();
    }
}