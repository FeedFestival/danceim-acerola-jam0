using Game.Shared.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IMotor {

        void SetUnitState(UnitState unitState);
    }

    public interface IFPSMotor: IMotor {
        public Vector2 Movement { get; set; }
        public bool Sprint { get; set; }
        public bool AnalogControl { get; set; }

        void Init(ICameraController cameraController, Animator animator);
    }

    public interface IRTSMotor : IMotor {

        void Init(Animator animator);
    }
}