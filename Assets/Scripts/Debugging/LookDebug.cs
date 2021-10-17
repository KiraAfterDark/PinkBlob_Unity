using System;
using UnityEngine;

namespace PinkBlob.Debugging
{
    public class LookDebug : MonoBehaviour
    {
        [SerializeField]
        private LayerMask layerMask;
        
        [SerializeField]
        private Color lineColor = Color.blue;
        
        [SerializeField]
        private Color hitPointColor = Color.red;

        [Min(0)]
        [SerializeField]
        private float hitPointRadius = 0.2f;

        [Min(0)]
        [SerializeField]
        private float maxLookDistance = 100f;

        public bool HasHit { get; private set; }
        
        public GameObject Hit { get; private set; }

        private void Update()
        {
            Transform trans = transform;
            
            Vector3 origin = trans.position;
            Vector3 forward = trans.forward;
            
            if (Physics.Raycast(origin, forward, out RaycastHit hit, maxLookDistance, layerMask))
            {
                HasHit = true;
                Hit = hit.collider.gameObject;
            }
        }
        
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Transform trans = transform;

                Vector3 origin = trans.position;
                Vector3 forward = trans.forward;

                if (Physics.Raycast(origin, forward, out RaycastHit hit, maxLookDistance, layerMask))
                {
                    Gizmos.color = lineColor;
                    Gizmos.DrawLine(origin, hit.point);

                    Gizmos.color = hitPointColor;
                    Gizmos.DrawSphere(hit.point, hitPointRadius);
                }
            }
        }
    }
}
