using Game.Shared.Constants;

namespace Game.Shared.Interfaces {
    public interface IPlayer {

        ICameraController CameraController { get; }
        IUnit Unit { get; }
        IUI UI { get; }
        GameState GameState { get; }
        PlayerState PlayerState { get; }
        IPlayerControl PlayerControl { get; }

        void SetGameState(GameState gameState);
        void SetControlState(PlayerState playerState);
        void SetInteractionControl(InteractionType interactionType);
    }
}