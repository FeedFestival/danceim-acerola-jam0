using Game.Shared.Constants;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnit {
        int ID { get; }
        string Name { get; }
        Transform Transform { get; }
        IActor Actor { get; }
        IUnitControl UnitControl { get; }

        void Init();
        void SetUnitState(UnitState unitState);
    }

    public interface IPlayerUnit: IUnit {
        Transform SpineToOrientate { get; }
    }

    public interface INPCUnit : IUnit {
    }
}
