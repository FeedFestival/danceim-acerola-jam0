using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections;
using UnityEngine;

namespace Game.Unit {
    public class PlayerUnit : Unit, IPlayerUnit {
        [Header("Player Unit")]
        [SerializeField]
        private Transform _spineToOrientate;
        private IEnumerator _waitCheckBelow;
        private float _waitCheckTime = 0.25f;
        [SerializeField]
        private LayerMask _zoneLayer;
        private IZone _lastZone;
        private IGameplayState _gameplayState;

        [SerializeField]
        private GameObject _keyInHand;
        public Transform SpineToOrientate { get => _spineToOrientate; }
        public IInventory Inventory { get; private set; }

        public void Init(IGameplayState gameplayState) {
            initEntityId();
            tryLoadActor();
            tryInitActor();

            Inventory = GetComponent<IInventory>();
            Inventory.Init();

            Inventory.InventoryItemAdded += onInventoryChanged;
            Inventory.InventoryItemRemoved += onInventoryChanged;

            (Actor as IPatientActor).Init(Inventory);

            UnitControl = GetComponent<IUnitControl>();

            _gameplayState = gameplayState;
            _gameplayState.OnGameplayRecalculation += recalculate;

            _keyInHand.SetActive(false);
        }

        //----------------------------------------------------------------------------------

        private void checkBelow() {
            stopCheckingBelow();

            var raycastOrigin = transform.position + Vector3.up * 0.5f;
            var ray = new Ray(raycastOrigin, Vector3.down);
            float maxRaycastDistance = 2.0f;

            if (Physics.Raycast(ray, out var hit, maxRaycastDistance, _zoneLayer)) {
                var zone = hit.transform.GetComponent<IZone>();
                if (zone != null) {
                    if (_lastZone != null) {
                        if (_lastZone.ID != zone.ID) {
                            _lastZone.EmitExited();
                            _lastZone = zone;
                            _lastZone.EmitEntered();
                        }
                    } else {
                        _lastZone = zone;
                        _lastZone.EmitEntered();
                    }
                }
                if (_lastZone != null) {
                    _lastZone.EmitExited();
                    _lastZone = null;
                }
                return;
            }

            _waitCheckBelow = waitCheckBelow();
            StartCoroutine(_waitCheckBelow);
        }

        private IEnumerator waitCheckBelow() {
            yield return new WaitForSeconds(_waitCheckTime);
            checkBelow();
        }

        private void stopCheckingBelow() {
            if (_waitCheckBelow != null) {
                StopCoroutine(_waitCheckBelow);
                _waitCheckBelow = null;
            }
        }

        private void recalculate() {
            if (_gameplayState.GameState == GameState.InLoading
                || _gameplayState.GameState == GameState.InMainMenu
                || _gameplayState.GameState == GameState.Paused) {

                stopCheckingBelow();

            } else if (_gameplayState.GameState == GameState.InGame) {
                checkBelow();

                if (_gameplayState.PlayerState == PlayerState.Playing
                    && Inventory.HasItem(InventoryItem.RightHand) == false) {
                    (UnitControl as IPlayerUnitControl).CanFire = true;
                } else {
                    (UnitControl as IPlayerUnitControl).CanFire = false;
                }
            }

            UnitControl.Motor.SetUnitState(_gameplayState.UnitState);
        }

        private void onInventoryChanged(InventoryItem inventoryItem) {
            if (Inventory.HasItem(InventoryItem.RightHand) == false) {
                (UnitControl as IPlayerUnitControl).CanFire = true;
            } else {
                (UnitControl as IPlayerUnitControl).CanFire = false;
            }

            if (Inventory.HasItem(InventoryItem.Key)) {
                _keyInHand.SetActive(true);
            } else if (Inventory.HasItem(InventoryItem.Key) == false) {
                _keyInHand.SetActive(false);
            }
        }
    }
}
