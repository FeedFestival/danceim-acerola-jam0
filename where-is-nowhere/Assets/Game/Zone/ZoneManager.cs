using Game.Shared.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Zone {
    public class ZoneManager : MonoBehaviour, IZoneManager {

        public Dictionary<int, IZone> Zones { get; private set; }

        public void Init() {
            Zones = new Dictionary<int, IZone>();

            foreach (Transform t in transform) {
                var zone = t.GetComponent<IZone>();

                if (zone == null) { continue; }

                zone?.Init();
                Zones.Add(zone.ID, zone);
            }
        }
    }
}