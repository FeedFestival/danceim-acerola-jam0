using Game.Shared.Constants;
using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface ICameraController {

        float RelativeYaw { get; }
        Transform Transform { get; }
        Action<int?> OnCameraFocussedInteractable { get; set; }

        void Init(IUnit unitRef);
        void SetCameraControl(CameraControl playerCameraState, bool enabled = true);
        int GetFocusedId();
        void SetVirtualCameraFocusTarget(
            Vector3? futurePos = null,
            Transform focusTarget = null
        );
        void SetCameraNoise(MotorState motorState);
    }
}