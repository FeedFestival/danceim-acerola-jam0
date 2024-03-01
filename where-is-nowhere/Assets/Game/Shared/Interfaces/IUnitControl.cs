using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IUnitControl {
        void Init(ICameraController cameraController, IActor actor);
        void Move(Vector2 move);
        void Sprint(bool value);
        void AnalogControl(bool value);
    }
}