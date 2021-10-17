using System;
using System.Numerics;
using PinkBlob.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace PinkBlob.Gameplay.Enemy
{
    public class LineOfSight : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Transform eyePosition;
        
        [Tooltip("Layers that block line of sight")]
        [SerializeField]
        private LayerMask vision;

        [Min(0)]
        [SerializeField]
        private float sightRange = 10f;

        [Range(0, 180)]
        [SerializeField]
        private float fieldOfView = 45f;

        public bool HasLineOfSight(ISeeable target)
        {
            Vector3 pos = eyePosition.position;
            Vector3 dir = target.GetSeePosition() - pos;

            float angle = Vector3.Angle(eyePosition.forward, dir);

            if (angle > fieldOfView)
            {
                return false;
            }
            
            var ray = new Ray(pos, dir);
            if (Physics.Raycast(ray, out RaycastHit hit, sightRange, vision) && hit.collider.TryGetComponent(out ISeeable see))
            {
                return see == target;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Vector3 forward = eyePosition.transform.forward;
            Vector3 leftForward = Quaternion.AngleAxis(-fieldOfView, Vector3.up) * forward;
            Vector3 rightForward = Quaternion.AngleAxis(fieldOfView, Vector3.up) * forward;

            Vector3 pos = eyePosition.position;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + leftForward * sightRange);
            Gizmos.DrawLine(pos, pos + rightForward * sightRange);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pos, 0.1f);
        }
        
        #region Debugging

        public void PrintDebug(ISeeable target)
        {
            EditorGUILayout.LabelField($"Has Line of Sight: {HasLineOfSightDebug(target)}");
        }

        private bool HasLineOfSightDebug(ISeeable target)
        {
            Vector3 pos = eyePosition.position;
            Vector3 dir = target.GetSeePosition() - pos;

            float angle = Vector3.Angle(eyePosition.forward, dir);

            EditorGUILayout.LabelField($"Angle to target: {angle}");
            if (angle > fieldOfView)
            {
                EditorGUILayout.LabelField($"Target outside Field of View: {fieldOfView}");
                return false;
            }
            
            var ray = new Ray(pos, dir);
            if (Physics.Raycast(ray, out RaycastHit hit, sightRange, vision))
            {
                EditorGUILayout.LabelField($"Sight collides with: {hit.collider.gameObject.name}");
                if (hit.collider.TryGetComponent(out ISeeable see))
                {
                    EditorGUILayout.LabelField($"Collision is seeable. Is target: {see == target}");
                    return see == target;
                }
                else
                {
                    EditorGUILayout.LabelField($"Collision is not seeable.");
                }
            }
            
            
            EditorGUILayout.LabelField($"Sight collides with: Nothing");
            return false;
        }
        
        #endregion
    }
}
