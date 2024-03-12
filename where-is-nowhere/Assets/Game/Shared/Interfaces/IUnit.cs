using Game.Shared.Constants;
using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnit {
        int ID { get; }
        string Name { get; }
        Transform Transform { get; }
        IActor Actor { get; }
        IUnitControl UnitControl { get; }

        void SetUnitState(UnitState unitState);
    }

    public interface IPlayerUnit: IUnit {
        Transform SpineToOrientate { get; }
        IInventory Inventory { get; }

        void Init(IGameplayState gameplayState);
    }

    public interface INPCUnit : IUnit {
        void Init(ITrigger movementTargetTrigger);
    }
}
