using System.Collections.Generic;

namespace Game.Shared.Interfaces {
    public interface IZoneManager {
        Dictionary<int, IZone> Zones { get; }

        void Init();
    }
}
