using Game.Shared.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IInventory {
        Action<InventoryItem> InventoryItemAdded { get; set; }
        Action<InventoryItem> InventoryItemRemoved { get; set; }

        void Init();
        void AddToInventory(InventoryItem inventoryItem);
        bool HasItem(InventoryItem inventoryItem);
        void ConsumeItem(InventoryItem inventoryItem);
    }
}