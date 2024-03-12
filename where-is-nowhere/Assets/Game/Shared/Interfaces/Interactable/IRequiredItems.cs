using Game.Shared.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IRequiredItems {
        InventoryItem[] RequiredItems { get; set; }
    }
}