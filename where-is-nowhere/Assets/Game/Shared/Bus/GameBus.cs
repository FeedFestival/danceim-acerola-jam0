using System;

namespace Game.Shared.Bus {
    public class GameBus : GSBus, IGameBus {
        public bool Emit(GameEvt evt, object data = null) {
            return base.Emit(evt, data);
        }
        public IEventPackage On(GameEvt evt, Action handler) {
            return base.On(evt, handler) as IEventPackage;
        }
        public IEventPackage On(GameEvt evt, Action<object> handler) {
            return base.On(evt, handler) as IEventPackage;
        }
        public void UnregisterByEvent(GameEvt evt) {
            base.UnregisterByEvent(evt);
        }
        public void UnregisterByEvent(GameEvt evt, Action handler) {
            base.UnregisterByEvent(evt, handler);
        }
        public void UnregisterByEvent(GameEvt evt, Action<object> handler) {
            base.UnregisterByEvent(evt, handler);
        }
    }
}
