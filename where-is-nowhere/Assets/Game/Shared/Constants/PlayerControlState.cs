namespace Game.Shared.Constants {
    public enum GameState {
        InMainMenu,
        InLoading,
        InGame,
    }

    public enum PlayerState {
        BrowsingMenu,
        Playing,
        Interacting,
        WatchingCutcene
    }

    public enum ControlPermission {
        ControlMovement,
        ControlLook,
        ControlGlobal
    }

    public enum UnitState {
        Hidden,
        FreePlaying,
        Interacting
    }
}
