using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Game.Player {
    public class GameplayState : IGameplayState {

        public GameState GameState { get; private set; }
        public PlayerState PlayerState { get; private set; }
        public UnitState UnitState { get; private set; }
        public InteractionType InteractionType { get; private set; }
        public Action OnGameplayRecalculation { get; set; }
        public Dictionary<CameraControl, bool> CameraControlPermissions { get; private set; }
        public Dictionary<ControlPermission, bool> ControlPermissions { get; private set; }
        private Subject<bool> _dispatchRecalculation = new Subject<bool>();

        public GameplayState() {
            CameraControlPermissions = new Dictionary<CameraControl, bool>() {
                {
                    CameraControl.Position,
                    false
                },
                {
                    CameraControl.Look,
                    false
                },
                {
                    CameraControl.Mouse,
                    false
                }
            };

            ControlPermissions = new Dictionary<ControlPermission, bool>() {
                {
                    ControlPermission.ControlGlobal,
                    false
                },
                {
                    ControlPermission.ControlLook,
                    false
                },
                {
                    ControlPermission.ControlMovement,
                    false
                }
            };

            _dispatchRecalculation
                .Throttle(TimeSpan.FromMilliseconds(50))
                .Do(_ => {
                    recalculate();
                    OnGameplayRecalculation?.Invoke();
                })
                .Subscribe();
        }

        public void SetState(
            GameState gameState,
            PlayerState playerState,
            bool emit = true
        ) {
            GameState = gameState;
            PlayerState = playerState;

            if (!emit) return;
            _dispatchRecalculation.OnNext(true);
        }

        public void SetState(
            GameState gameState,
            PlayerState playerState,
            UnitState unitState,
            bool emit = true
        ) {
            SetState(gameState, playerState, false);
            UnitState = unitState;

            if (!emit) return;
            _dispatchRecalculation.OnNext(true);
        }

        public void SetState(
            GameState gameState,
            PlayerState playerState,
            UnitState unitState,
            InteractionType interactionType,
            bool emit = true
        ) {
            SetState(gameState, playerState, unitState, false);
            InteractionType = interactionType;

            if (!emit) return;
            _dispatchRecalculation.OnNext(true);
        }

        public void ForceRecalculation() {
            _dispatchRecalculation.OnNext(true);
        }

        private void recalculate() {

            switch (GameState) {
                case GameState.InMainMenu:
                    EnableCameraControl(CameraControl.Look, false);
                    EnableCameraControl(CameraControl.Position, false);
                    EnableCameraControl(CameraControl.Mouse, false);
                    EnableControlPermission(ControlPermission.ControlLook, false);
                    EnableControlPermission(ControlPermission.ControlMovement, false);
                    EnableControlPermission(ControlPermission.ControlGlobal, false);

                    break;
                case GameState.InLoading:

                    EnableCameraControl(CameraControl.Look, false);
                    EnableCameraControl(CameraControl.Position, false);
                    EnableCameraControl(CameraControl.Mouse, false);
                    EnableControlPermission(ControlPermission.ControlLook, false);
                    EnableControlPermission(ControlPermission.ControlMovement, false);
                    EnableControlPermission(ControlPermission.ControlGlobal, false);

                    break;
                case GameState.InGame:

                    switch (PlayerState) {
                        case PlayerState.BrowsingMenu:

                            EnableCameraControl(CameraControl.Look, false);
                            EnableControlPermission(ControlPermission.ControlLook, false);
                            EnableControlPermission(ControlPermission.ControlMovement, false);
                            EnableControlPermission(ControlPermission.ControlGlobal);

                            break;
                        case PlayerState.WatchingCutcene:

                            break;
                        case PlayerState.Interacting:

                            switch (InteractionType) {
                                case InteractionType.None:
                                    break;
                                case InteractionType.WorldSelection:

                                    EnableCameraControl(CameraControl.Position);
                                    EnableCameraControl(CameraControl.Look, false);
                                    EnableCameraControl(CameraControl.Mouse);
                                    EnableControlPermission(ControlPermission.ControlLook, false);
                                    EnableControlPermission(ControlPermission.ControlMovement, false);
                                    EnableControlPermission(ControlPermission.ControlGlobal);

                                    break;
                                case InteractionType.UISelection:

                                    break;
                                case InteractionType.Default:
                                default:

                                    EnableCameraControl(CameraControl.Position);
                                    EnableCameraControl(CameraControl.Look, false);
                                    EnableCameraControl(CameraControl.Mouse, false);
                                    EnableControlPermission(ControlPermission.ControlLook, false);
                                    EnableControlPermission(ControlPermission.ControlMovement, false);
                                    EnableControlPermission(ControlPermission.ControlGlobal);

                                    break;
                            }

                            break;
                        case PlayerState.Playing:
                        default:

                            EnableCameraControl(CameraControl.Position);
                            EnableCameraControl(CameraControl.Look);
                            EnableCameraControl(CameraControl.Mouse, false);
                            EnableControlPermission(ControlPermission.ControlLook);
                            EnableControlPermission(ControlPermission.ControlMovement);
                            EnableControlPermission(ControlPermission.ControlGlobal);

                            break;
                    }

                    break;
                case GameState.Paused:
                default:

                    EnableCameraControl(CameraControl.Look, false);
                    EnableCameraControl(CameraControl.Position, false);
                    EnableCameraControl(CameraControl.Mouse, false);
                    EnableControlPermission(ControlPermission.ControlLook, false);
                    EnableControlPermission(ControlPermission.ControlMovement, false);
                    EnableControlPermission(ControlPermission.ControlGlobal, false);

                    break;

            }
        }

        private void EnableCameraControl(CameraControl playerCameraState, bool enabled = true) {
            if (CameraControlPermissions[playerCameraState] == enabled) {
                return;
            }
            CameraControlPermissions[playerCameraState] = enabled;
        }

        private void EnableControlPermission(ControlPermission controlPermission, bool enabled = true) {

            if (ControlPermissions[controlPermission] == enabled) {
                return;
            }
            ControlPermissions[controlPermission] = enabled;
        }
    }
}