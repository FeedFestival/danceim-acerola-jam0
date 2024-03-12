using Game.Shared.Constants;
using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IMotor {
        public Action<int> DestinationReached { get; set; }

        void SetUnitState(UnitState unitState);
        void Teleport(Vector3 position, bool onNavMesh = false, bool smooth = false);
    }

    public interface IFPSMotor: IMotor {
        public Vector2 Movement { get; set; }
        public bool Sprint { get; set; }
        public bool AnalogControl { get; set; }

        void Init(ICameraController cameraController, Animator animator);
    }

    public interface IRTSMotor : IMotor {

        void Init(int id, Animator animator, ITrigger movementTargetTrigger);
    }
}