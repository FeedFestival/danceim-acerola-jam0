using Game.Shared.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {
    public class RightHandUnit : NPCUnit, IRightHandUnit {

        [Header("Right Hand Unit")]
        [SerializeField]
        private PossiblePositions _possiblePositions;

        public override void Init(ITrigger movementTargetTrigger) {
            base.Init(movementTargetTrigger);

            _possiblePositions.Init();
        }

        public void MoveRandomly() {
            var randomPoint = _possiblePositions.GetRandomPoint();
            (UnitControl as INPCControl).MoveTo(randomPoint);
        }
    }
}