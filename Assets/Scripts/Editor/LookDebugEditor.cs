using PinkBlob.Debugging;
using UnityEditor;
using UnityEngine;

namespace PinkBlob.Editor
{
    [CustomEditor(typeof(LookDebug))]
    public class LookDebugEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var lookDebug = (LookDebug) target;
            GUILayout.Label($"Looking at object: {lookDebug.HasHit}");
            if (lookDebug.HasHit)
            {
                GUILayout.Label($"Object: {lookDebug.Hit.name}");
            }
        }
    }
}
