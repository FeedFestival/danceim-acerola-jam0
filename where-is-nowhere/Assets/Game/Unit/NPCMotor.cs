using DG.Tweening;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Unit {
    public class NPCMotor : MonoBehaviour, IRTSMotor {

        [SerializeField]
        private LayerMask _raycastLayer;

        private NavMeshAgent _navMeshAgent;
        private Animator _animatorRef;

        private IDisposable _motorLateUpdateObs;
        private ITrigger _movementTargetTrigger;
        private Tween _speedTween;
        private Tween _rotateTween;
        private Vector3? _nextCornerPos;
        private float _animatorSpeed;
        private int _id;

        public Action<int> DestinationReached { get; set; }

        public void Init(int id, Animator animator, ITrigger movementTargetTrigger) {

            _id = id;
            _animatorRef = animator;

            _movementTargetTrigger = movementTargetTrigger;
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.enabled = true;
            _navMeshAgent.radius = 0.01f;
            _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            DestinationReached += destinationReached;
        }

        public void SetUnitState(UnitState unitState) { }

        internal void MovementTargetChanged(Vector3 targetPos) {
            stayOnNavMesh(ref targetPos);
            moveToTarget(ref targetPos);
        }

        //----------------------------------------------------------------------------------------

        void OnDestroy() {
            killSpeedTweenIfRunning();
            _motorLateUpdateObs?.Dispose();
        }

        //----------------------------------------------------------------------------------------

        private void stayOnNavMesh(ref Vector3 pos) {
            NavMeshHit hit;
            bool canWalk = NavMesh.SamplePosition(pos, out hit, 20, NavMesh.AllAreas);
            if (canWalk) {
                pos = hit.position;
            } else {
                if (NavMesh.FindClosestEdge(pos, out hit, NavMesh.AllAreas)) {
                    pos = hit.position;
                }
            }
        }

        private void moveToTarget(ref Vector3 pos) {

            float distance = Vector3.Distance(pos, transform.position);
            if (distance < 0.7f) { return; }

            if (_navMeshAgent.isStopped) {
                _navMeshAgent.isStopped = false;
            }

            _movementTargetTrigger.transform.position = pos;
            _movementTargetTrigger.Enable();
            _navMeshAgent.SetDestination(pos);

            _motorLateUpdateObs?.Dispose();
            _motorLateUpdateObs = this.LateUpdateAsObservable()
                .ThrottleFirst(TimeSpan.FromMilliseconds(100))
                .Do(_ => changeSpeed(_navMeshAgent.velocity.magnitude, 0.1f))
                .Do(_ => rotateTowardsNextPointOnNavMesh())
                .Do(_ => checkIfReachedDestination())
                .Subscribe();
        }

        private void changeSpeed(float toSpeed, float time) {
            killSpeedTweenIfRunning();

            _speedTween = DOTween
            .To(() => _animatorSpeed,
                speed => _animatorSpeed = speed,
                toSpeed,
                time
            )
            .SetEase(Ease.Linear)
            .OnUpdate(() => {
                _animatorRef.SetFloat("MoveSpeed", toSpeed / _animatorSpeed);
                _animatorRef.SetFloat("AnimSpeed", _animatorSpeed, time, Time.deltaTime);
            })
            .OnKill(() => { _speedTween = null; });
        }

        private void rotateTowardsNextPointOnNavMesh() {

            Vector3? nextCornerPos;

            try {
                nextCornerPos = _navMeshAgent.path.corners[1];
            } catch (Exception) {
#if UNITY_EDITOR
                var s = string.Empty;
                foreach (var item in _navMeshAgent.path.corners) {
                    s += item + "\n";
                }
                //Debug.Log("_navMeshAgent.path.corners: " + s);
#endif
                nextCornerPos = null;
            }

            if (nextCornerPos.HasValue == false || _nextCornerPos == nextCornerPos) return;

            _nextCornerPos = nextCornerPos;

            var lookAt = Quaternion.LookRotation(nextCornerPos.Value - transform.position);

            _rotateTween?.Kill();

            _rotateTween = DOVirtual
            .Float(0, 1, 0.33f, (value) => {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, value);
            });
        }

        private void checkIfReachedDestination() {

            if (!_navMeshAgent.pathPending) {
                if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) {
                    if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f) {
                        DestinationReached?.Invoke(_id);
                        return;
                    }
                }

                var raycastOrigin = transform.position + Vector3.up * 0.5f;
                var ray = new Ray(raycastOrigin, Vector3.down);
                float maxRaycastDistance = 2.0f;

                if (Physics.Raycast(ray, out var hit, maxRaycastDistance, _raycastLayer)) {
                    DestinationReached?.Invoke(_id);
                    return;
                } else {
                    Debug.DrawLine(raycastOrigin, raycastOrigin + Vector3.down * maxRaycastDistance, Color.green);
                }
            }
        }

        private void destinationReached(int id) {
            _motorLateUpdateObs?.Dispose();
            _navMeshAgent.isStopped = true;

            changeSpeed(0, 0.3f);
        }

        private void killSpeedTweenIfRunning() {
            if (_speedTween != null && _speedTween.IsActive()) {
                _speedTween.Kill();
                _speedTween = null;
            }
        }
    }
}