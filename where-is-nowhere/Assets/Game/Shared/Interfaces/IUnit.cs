using Game.Shared.Constants;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnit {
        int ID { get; }
        string Name { get; }
        Transform Transform { get; }
        IActor Actor { get; }
        IUnitControl UnitControl { get; }
        Transform SpineToOrientate { get; }

        void Init();
        void SetUnitState(UnitState unitState);
    }
}
