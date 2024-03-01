using System;

namespace Game.Shared.Bus {
    public interface IGameBus {
        bool Emit(GameEvt evt, object data = null);
        IEventPackage On(GameEvt evt, Action handler);
        IEventPackage On(GameEvt evt, Action<object> handler);
        void UnregisterByEvent(GameEvt evt);
        void UnregisterByEvent(GameEvt evt, Action handler);
        void UnregisterByEvent(GameEvt evt, Action<object> handler);
    }
}
