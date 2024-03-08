using DG.Tweening;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Unit {
    public class Motor : MonoBehaviour, IFPSMotor {
        [Header("Settings")]
        [Tooltip("NavMeshAgent.BaseOffset + 0.14")]
        [SerializeField]
        private float _walkSpeed = 1;
        [SerializeField]
        private float _moveSpeedMultiplier = 1.8f;
        [SerializeField]
        private float _sprintSpeedMultiplier = 5.335f;

        public Vector2 Movement { get; set; }
        public bool Sprint { get; set; }
        public bool AnalogControl { get; set; }

        public float ForwardAmount;
        public float TurnAmount;

        private ICameraController _cameraController;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private readonly float _groundCheckDistance = 1.22f;
        private MotorState _motorState;
        private Vector3 _groundNormal;

        private void Update() {

            animatorMove();

            if (Movement.sqrMagnitude >= .01f) {

                // Calculate movement direction based on camera rotation
                var forward = _cameraController.Transform.forward;
                var right = _cameraController.Transform.right;
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                // Calculate movement vector
                var moveDirection = forward * Movement.y + right * Movement.x;

                // Apply movement to the character controller
                var speedMultiplier = Sprint ? _sprintSpeedMultiplier : _moveSpeedMultiplier;
                var agentMoveDir = moveDirection * speedMultiplier * Time.deltaTime;

                var newPosition = transform.position + agentMoveDir;
                if (NavMesh.SamplePosition(newPosition, out var hit, _groundCheckDistance, NavMesh.AllAreas)) {
                    _navMeshAgent.Move(agentMoveDir);
                }
            }
        }

        private void FixedUpdate() {
            calculateMovement();
        }

        private void LateUpdate() {
            setMotorState();
        }

        //------------------------------------------------------------------------------------------------

        public void Init(ICameraController cameraController, Animator animator) {
            _cameraController = cameraController;
            _animator = animator;

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.updateRotation = false;
        }

        public void SetUnitState(UnitState unitState) {

            Debug.Log("unitState: " + unitState);

            switch (unitState) {
                case UnitState.FreePlaying:

                    _navMeshAgent.radius = 0.5f;
                    _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

                    break;
                case UnitState.Interacting:

                    _navMeshAgent.radius = 0.01f;
                    _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                    break;
                case UnitState.Hidden:
                default:

                    _navMeshAgent.radius = 0.01f;
                    _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                    break;
            }
        }

        //------------------------------------------------------------------------------------------------

        internal void Teleport(Vector3 position, bool onNavMesh = false, bool smooth = false) {
            if (onNavMesh) {
                NavMeshHit closestHit;
                if (NavMesh.SamplePosition(position, out closestHit, 500f, NavMesh.AllAreas)) {
                    position = closestHit.position;
                } else {
                    Debug.LogError("Could not find position on NavMesh!");
                }
            }

            var correctedPos = position + new Vector3(0, _navMeshAgent.baseOffset, 0);
            if (smooth) {
                transform.DOMove(correctedPos, 0.66f).SetEase(Ease.OutQuint);
            } else {
                transform.position = correctedPos;
            }
        }

        private void calculateMovement() {

            var camForward = Vector3.Scale(_cameraController.Transform.forward, new Vector3(1, 0, 1)).normalized;
            var move = Movement.y * camForward * _walkSpeed;
            if (move.magnitude > 1f) {
                move.Normalize();
            }

            if (Sprint) {
                move *= 2;
            }

            move = Vector3.ProjectOnPlane(transform.InverseTransformDirection(move), _groundNormal);

            ForwardAmount = Mathf.Abs(move.z);
        }

        private void setMotorState() {

            var motorState = MotorState.Idle;
            if (Movement.sqrMagnitude >= 0.01f) {
                motorState = MotorState.Moving;
                if (Sprint) {
                    motorState = MotorState.Sprinting;
                }
            }/* else {
                motorState = MotorState.Crouching;
            }*/

            if (_motorState == motorState) {
                return;
            }
            _motorState = motorState;

            _cameraController.SetCameraNoise(_motorState);
        }

        private void animatorMove() {

            // ----- TURN               ------

            TurnAmount = _cameraController.RelativeYaw;
            transform.eulerAngles = new Vector3(0, _cameraController.Transform.eulerAngles.y, 0);
            _animator.SetFloat("Turn", TurnAmount * 0.3f, 0.1f, Time.deltaTime);

            // ----- FORWARDS           ------

            _animator.SetFloat("Forward", ForwardAmount, 0.1f, Time.deltaTime);

            float animDir = Movement.y >= 0 ? 1 : -1;
            _animator.SetFloat("animDir", animDir, 0.1f, Time.deltaTime);

        }
    }
}
