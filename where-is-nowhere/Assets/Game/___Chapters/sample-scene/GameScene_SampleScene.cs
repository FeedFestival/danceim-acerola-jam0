using DG.Tweening;
using Game.Scene;
using Game.Shared.Bus;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Chapters {
    public class GameScene_SampleScene : GameScene {

        private const string _sceneName = "awakening";
        private const string _nextSceneName = "end";
        private IEnumerator _nextScene;

        [SerializeField]
        private Transform _startPoint;

        [SerializeField]
        private AudioSource _otherPatientAudioSource;

        public override void StartScene() {

            ftLightmaps.RefreshFull();
            LightProbes.Tetrahedralize();

            PlayerInteracted += playerInteracted;
            PlayerInteractedUnit += playerInteractedUnit;
            PlayerAttackedUnit += playerAttackedUnit;

            _player.Unit.Transform.position = _startPoint.position;
            _player.Unit.UnitControl.Teleport(_startPoint.position);

            _player.GameplayState.SetState(
                GameState.InGame,
                PlayerState.Playing,
                UnitState.FreePlaying,
                InteractionType.None
            );

            __.GameBus.Emit(GameEvt.PLAY_AMBIENT, AmbientSFXName.Sanatorium);

            foreach (var unit in _unitManager.Units) {
                (unit.Value.UnitControl as INPCControl).Motor.DestinationReached += onDestinationReached;
                onDestinationReached(unit.Key);
            }

            setupInteractions();
        }

        private void playerInteracted(int entityId) {
            var interactable = _interactableManager.Interactables[entityId];

            if ((interactable as IRequiredItems) != null
                && (interactable as IRequiredItems).RequiredItems != null
                && (interactable as IRequiredItems).RequiredItems.Length > 0) {

                var itemMatch = new List<InventoryItem>();
                for (int i = 0; i < (interactable as IRequiredItems).RequiredItems.Length; i++) {
                    var requiredItem = (interactable as IRequiredItems).RequiredItems[i];
                    var hasItem = (_player.Unit as IPlayerUnit).Inventory.HasItem(requiredItem);
                    if (hasItem) {
                        itemMatch.Add(requiredItem);
                    }
                }

                if (itemMatch.Count == 0) { return; }
                if (itemMatch.Count != (interactable as IRequiredItems).RequiredItems.Length) { return; }

                var lockableInteractable = interactable as ILockable;
                if (lockableInteractable != null) {
                    if (lockableInteractable.IsLocked) {
                        return;
                    }
                }

                if (entityId != 200001) {
                    foreach (var itm in itemMatch) {

                        (_player.Unit as IPlayerUnit).Inventory.ConsumeItem(itm);
                        (_player.GameplayState).ForceRecalculation();

                        if (itm == InventoryItem.RightHand) {
                            _unitManager.Units[101].UnitControl.Teleport(interactable.Transform.position, smooth: true);
                            _unitManager.Units[101].SetUnitState(UnitState.FreePlaying);
                            onDestinationReached(101);
                        }
                    }
                }
            }

            (interactable as IDefaultInteraction).DoDefaultInteraction(_player);
        }

        private void playerInteractedUnit(int entityId) {
            Debug.Log("playerInteractedUnit.entityId: " + entityId);
            if (entityId == 101) {
                _player.PlayerControl.FirePerformed?.Invoke();
            } else {
                (_unitManager.Units[entityId] as IDefaultInteraction).DoDefaultInteraction(_player);
            }
        }

        private void playerAttackedUnit(int entityId) {
            (_unitManager.Units[entityId] as IDefaultInteraction).DoDefaultInteraction(_player);
        }

        private void onDestinationReached(int id) {
            (_unitManager.Units[id] as IRightHandUnit).MoveRandomly();
        }

        private void setupInteractions() {

            var requiredItems = new InventoryItem[1] { InventoryItem.RightHand };

            var doorsThatNeedHands = new int[9] { 200001, 300002, 300003, 300005, 300006, 300007, 300008, 300009, 300010 };
            foreach (int id in doorsThatNeedHands) {
                (_interactableManager.Interactables[id] as IRequiredItems).RequiredItems = requiredItems;
            }

            (_interactableManager.Interactables[300004] as IRequiredItems).RequiredItems
                = new InventoryItem[2] { InventoryItem.RightHand, InventoryItem.Key };
            _interactableManager.Interactables[300004].OnInteracted += () => {

                DOVirtual
                    .Float(0, 1, 1, (float value) => { })
                    .OnComplete(() => {
                        _otherPatientAudioSource.clip = (_soundManager as ISFXSoundManager)?.GetSoundFxClip(SFXName.CrazyDudeScared);
                        _otherPatientAudioSource.loop = false;
                        _otherPatientAudioSource.volume = 0;

                        DOVirtual
                            .Float(0, 1, _otherPatientAudioSource.clip.length, (float value) => {
                                _otherPatientAudioSource.volume = value;
                            })
                            .SetEase(Ease.InSine)
                            .OnComplete(() => {
                                _otherPatientAudioSource.clip = (_soundManager as ISFXSoundManager)?.GetSoundFxClip(SFXName.CrazyDudeBreathing);
                                _otherPatientAudioSource.loop = true;
                            });
                    });
            };

            _zoneManager.Zones[100001].Entered += goNextScene;

            // Other patient
            _otherPatientAudioSource.volume = 0.05f;
            _zoneManager.Zones[100007].Entered += () => {
                _otherPatientAudioSource.volume = 1;
            };
            _zoneManager.Zones[100007].Exited += () => {
                _otherPatientAudioSource.volume = 0.05f;
            };

            (_interactableManager.Interactables[300007] as ISolvable).OnSolved += () => {
                (_interactableManager.Interactables[300008] as ILockable).Lock(false);
            };

            _interactableManager.Interactables[300006].OnInteracted += () => {
                Debug.Log("Interacted with simple door");
            };

            _interactableManager.Interactables[200001].OnInteracted += () => {
                Debug.Log("INTERACTED with simple <b>KEY</b>");

                (_player.Unit as IPlayerUnit).Inventory.AddToInventory(InventoryItem.Key);
            };
        }

        private void goNextScene() {

            _nextScene = goEndGameScene();
            StartCoroutine(_nextScene);
        }

        private IEnumerator goEndGameScene() {

            _player.GameplayState.SetState(
                GameState.InLoading,
                PlayerState.BrowsingMenu,
                UnitState.Hidden,
                InteractionType.None
            );

            yield return new WaitForSeconds(0.1f);

            var loadSceneParameters = new LoadSceneParameters();
            loadSceneParameters.loadSceneMode = LoadSceneMode.Additive;

            var scene = SceneManager.LoadScene(_nextSceneName, loadSceneParameters);

            while (!scene.isLoaded) {
                yield return null;
            }

            // Ensure that the newly loaded scene is set as the active scene
            var loadedScene = SceneManager.GetSceneByName(_nextSceneName);
            //var loadedScene = SceneManager.GetSceneByBuildIndex(1);
            if (loadedScene.IsValid() && loadedScene.isLoaded) {
                SceneManager.SetActiveScene(loadedScene);
            } else {
                Debug.LogError("Failed to set the active scene.");
            }

            SceneManager.UnloadSceneAsync(_sceneName);

            StopCoroutine(_nextScene);
        }
    }
}