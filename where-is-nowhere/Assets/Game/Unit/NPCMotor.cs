using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Unit {
    public class NPCMotor : MonoBehaviour, IRTSMotor {

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        public void Init(Animator animator) {
            _animator = animator;

            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetUnitState(UnitState unitState) {

            Debug.Log("unitState: " + unitState);

            switch (unitState) {
                case UnitState.FreePlaying:

                    _navMeshAgent.radius = 0.5f;
                    _navMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;

                    break;
                case UnitState.Interacting:

                    _navMeshAgent.radius = 0.01f;
                    _navMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;

                    break;
                case UnitState.Hidden:
                default:

                    _navMeshAgent.radius = 0.01f;
                    _navMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;

                    break;
            }
        }
    }
}