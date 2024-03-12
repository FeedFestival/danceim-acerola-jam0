using Game.Shared.Constants;
using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface ICameraController {

        float RelativeYaw { get; }
        Transform Transform { get; }
        Action<int?> OnCameraFocussedInteractable { get; set; }

        void Init(IGameplayState gameplayState, IUnit unitRef);
        IFocusTrigger GetFocusedTrigger();
        void SetVirtualCameraFocusTarget(
            Vector3? futurePos = null,
            Transform focusTarget = null
        );
        void SetCameraNoise(MotorState motorState);
    }
}