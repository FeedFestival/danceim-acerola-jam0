namespace Game.Shared.Bus {
    public static class __ {
        public static IGameBus GameBus {
            get {
                if (_gameBus == null) {
                    _gameBus = new GameBus();
                }
                return _gameBus;
            }
        }
        private static IGameBus _gameBus;

        public static void ClearAll() {
            _gameBus = null;
        }
    }
}
