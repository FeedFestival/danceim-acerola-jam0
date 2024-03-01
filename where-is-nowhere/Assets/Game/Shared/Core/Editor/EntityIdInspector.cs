using UnityEditor;
using UnityEngine;

namespace Game.Shared.Core {
    [CustomEditor(typeof(EntityId))]
    public class EntityIdInspector : Editor {
        public override void OnInspectorGUI() {
            var script = (EntityId)target;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.yellow;
            GUILayout.Box("\n(Actor)         0 - 999\n" +
                          "> (Zone)        100000\n" +
                          "> (Item)        200000\n" +
                          "> (Interactable)  300000\n", GUILayout.MinWidth(300));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            GUILayout.Space(10);
            base.OnInspectorGUI();
        }
    }
}