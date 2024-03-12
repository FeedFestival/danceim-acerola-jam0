using Game.Shared.Constants;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnitControl {
        IMotor Motor { get; }

        void Sprint(bool value);
        void Teleport(Vector3 position, bool smooth = false);
    }

    public interface IPlayerUnitControl: IUnitControl {
        void Init(ICameraController cameraController, IActor actor);
        void Move(Vector2 move);
        void AnalogControl(bool value);
    }

    public interface INPCControl : IUnitControl {
        void Init(int id, IActor actor, ITrigger movementTargetTrigger);
        void MoveTo(Vector3 vector3);
    }
}