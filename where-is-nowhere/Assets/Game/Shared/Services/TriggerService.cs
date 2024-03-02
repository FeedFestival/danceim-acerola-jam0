using System;
using UnityEngine;

namespace Game.Shared.Services {

    public static class TriggerService {

        public static void SetupSharedMaterialForHighlight(ref Renderer renderer, ref int highlightIndex) {
            var materials = new Material[renderer.sharedMaterials.Length + 1];
            for (int i = 0; i < materials.Length - 1; i++) {
                materials[i] = renderer.sharedMaterials[i];
            }
            highlightIndex = materials.Length - 1;
            materials[highlightIndex] = null;
            renderer.sharedMaterials = materials;
        }

        public static void ChangeSharedMaterial(ref Renderer renderer, ref int index, Material material = null) {
            var sharedMaterials = renderer.sharedMaterials;
            sharedMaterials[index] = material;
            renderer.sharedMaterials = sharedMaterials;
        }
    }
}
