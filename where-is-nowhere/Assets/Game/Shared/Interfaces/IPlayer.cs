using Game.Shared.Constants;

namespace Game.Shared.Interfaces {
    public interface IPlayer {

        ICameraController CameraController { get; }
        IUnit Unit { get; }
        IUI UI { get; }
        IGameplayState GameplayState { get; }
        IPlayerControl PlayerControl { get; }
    }
}