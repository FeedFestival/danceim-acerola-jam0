using System.Collections.Generic;

namespace Game.Shared.Interfaces {
    public interface IUnitManager {
        Dictionary<int, IUnit> Units { get; }

        void Init();
    }
}