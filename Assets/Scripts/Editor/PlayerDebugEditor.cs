using PinkBlob.Gameplay;
using PinkBlob.Gameplay.Ability;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace PinkBlob.Editor
{
    public class PlayerDebugEditor : EditorWindow
    {
        [MenuItem("PinkBlob/Debug/Player")]
        static void Init()
        {
            var window = (PlayerDebugEditor)GetWindow(typeof(PlayerDebugEditor), false, "Player Debugging");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Player Debug", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                GUILayout.Label("Must be running", EditorStyles.helpBox);
            }
            else if (GameplayController.Instance != null && GameplayController.Instance.PlayerController != null)
            {
                GUILayout.Label($"Input: {GameplayController.Instance.PlayerController.MovementInput}");
                
                GUILayout.Space(5);
                
                GUILayout.Label($"Is Flying: {GameplayController.Instance.PlayerController.IsFlying}");
                GUILayout.Label($"Is Grounded: {GameplayController.Instance.PlayerController.IsGrounded}");
                GUILayout.Label($"Is Crouched: {GameplayController.Instance.PlayerController.IsCrouched}");

                GUILayout.Space(5);
                
                GUILayout.Label($"Max Speed: {GameplayController.Instance.PlayerController.MaxSpeed}");
                GUILayout.Label($"Acceleration: {GameplayController.Instance.PlayerController.Accel}");
                GUILayout.Label($"Deceleration: {GameplayController.Instance.PlayerController.Decel}");
                GUILayout.Label($"Velocity: {GameplayController.Instance.PlayerController.Velocity}");
                GUILayout.Label($"Speed: {GameplayController.Instance.PlayerController.Velocity.magnitude}");
                
                GUILayout.Space(5);
                
                GUILayout.Label($"Remaining Jumps: {GameplayController.Instance.PlayerController.RemainingJumps}");
                
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GameplayController.Instance.PlayerController.Ability.PrintDebugWindow();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }

        private void Update()
        {
            if (Application.isPlaying && GameplayController.Instance != null && GameplayController.Instance.PlayerController != null)
            {
                Repaint();
            }
        }
    }
}
