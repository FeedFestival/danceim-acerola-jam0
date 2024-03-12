using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory {
    public class Inventory : MonoBehaviour, IInventory {

        public Action<InventoryItem> InventoryItemAdded { get; set; }
        public Action<InventoryItem> InventoryItemRemoved { get; set; }

        private List<InventoryItem> _inventoryItems;

        public void Init() {
            _inventoryItems = new List<InventoryItem>();
        }

        public void AddToInventory(InventoryItem inventoryItem) {
            //Debug.Log("INVENTORY -> Add : " + inventoryItem);
            _inventoryItems.Add(inventoryItem);
            InventoryItemAdded?.Invoke(inventoryItem);
        }

        public bool HasItem(InventoryItem inventoryItem) {
            return _inventoryItems.Contains(inventoryItem);
        }

        public bool HasAnyItem() {
            return _inventoryItems.Count > 0;
        }

        public void ConsumeItem(InventoryItem inventoryItem) {
            if (HasItem(inventoryItem)) {
                //Debug.Log("INVENTORY -> Remove : " + inventoryItem);
                _inventoryItems.Remove(inventoryItem);
                InventoryItemRemoved?.Invoke(inventoryItem);
            }
        }
    }
}