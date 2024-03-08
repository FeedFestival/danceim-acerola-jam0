using Game.Shared.Constants;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnitControl {
        void Sprint(bool value);
        void SetUnitState(UnitState unitState);
    }

    public interface IPlayerUnitControl: IUnitControl {
        void Init(ICameraController cameraController, IActor actor);
        void Move(Vector2 move);
        void AnalogControl(bool value);
    }

    public interface INPCControl : IUnitControl {
        void Init(IActor actor);
    }
}