using PinkBlob.Gameplay;
using PinkBlob.Gameplay.Ai.StateMachine;
using PinkBlob.Gameplay.Enemy;
using PinkBlob.Gameplay.Suck;
using UnityEditor;
using UnityEngine;

namespace PinkBlob.Editor
{
    public class EnemyDebugEditor : EditorWindow
    {
        private bool transitionFold = false;
        private bool stateFold = false;
        private bool stateMachineFold = false;
        
        [MenuItem("PinkBlob/Debug/Enemy")]
        static void Init()
        {
            var window = (EnemyDebugEditor)GetWindow(typeof(EnemyDebugEditor), false, "Enemy Debugging");
            window.Show();
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Enemy Debug", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                GUILayout.Label("Must be running", EditorStyles.helpBox);
            }
            else if (GameplayController.Instance != null && GameplayController.Instance.PlayerController != null)
            {
                if (Selection.activeGameObject && Selection.activeGameObject.TryGetComponent(out EnemyController enemy))
                {
                    GUILayout.Label($"{enemy.name}");

                    if (enemy is ISuckable suckable)
                    {
                        GUILayout.Label($"Is being sucked: {suckable.IsSucking()}");
                    }

                    stateMachineFold = EditorGUILayout.Foldout(stateMachineFold, "State Machine", true);
                    if (stateMachineFold)
                    {
                        EditorGUI.indentLevel++;
                        enemy.StateMachine.PrintDebug();
                        EditorGUI.indentLevel--;
                    }

                    stateFold = EditorGUILayout.Foldout(stateFold, $"{enemy.StateMachine.CurrentState.StateName()}", true);
                    if (stateFold)
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.Label(enemy.StateMachine.CurrentState.Print());
                        EditorGUI.indentLevel--;
                    }

                    GUILayout.Space(5);

                    transitionFold = EditorGUILayout.Foldout(transitionFold, "Transitions", true);
                    if (transitionFold)
                    {
                        EditorGUI.indentLevel++;
                        foreach (Transition transition in enemy.StateMachine.Transitions)
                        {
                            if (enemy.StateMachine.CurrentState == transition.FromState)
                            {
                                EditorGUILayout
                                   .LabelField($"{transition.FromState.StateName()} <="
                                             + $" {transition.FromState.CanTransitionOut()} {transition.ToState.CanTransitionIn()} {transition.ExtraChecks()} "
                                             + $"=> {transition.ToState.StateName()}");
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    GUILayout.Label("Must select enemy", EditorStyles.helpBox);
                }
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
