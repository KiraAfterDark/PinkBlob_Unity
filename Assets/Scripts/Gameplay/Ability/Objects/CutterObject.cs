using System;
using System.Collections;
using System.Security.Cryptography;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability.Objects
{
    public class CutterObject : MonoBehaviour
    {
        private bool isInit = false;
        
        [Title("Movement")]
        
        [SerializeField]
        private float speed = 15f;

        private Vector3 velocity;

        [SerializeField]
        private float decel = 5f;

        [Min(0)]
        [SerializeField]
        private float rotationSpeed = 20;

        [Title("Visuals")]
        
        [Required]
        [SerializeField]
        private Transform visual;

        [Title("Hit")]
        
        [SerializeField]
        private Vector3 halfExtends = Vector3.one;

        [SerializeField]
        private LayerMask hitMask;

        private GameObject spawner;
        private bool canHitSpawner;
        
        private Vector3 moveVector;

        [Min(0)]
        [SerializeField]
        private float spawnerHitDelay = 0.2f;

        private int damage;
        
        [Title("Debug")]
        
        [SerializeField]
        private float debugDistance = 1f;

        public void Init(GameObject spawner, Vector3 velocity, int damage)
        {
            this.velocity = velocity + transform.forward * speed;
            this.spawner = spawner;
            this.damage = damage;

            StartCoroutine(SpawnerHitDelay());
            
            isInit = true;
        }

        private void Update()
        {
            if (isInit)
            {
                UpdatePosition();

                Vector3 rot = visual.localEulerAngles;
                rot.y += rotationSpeed * Time.deltaTime;
                visual.localEulerAngles = rot;
            }
        }

        private void UpdatePosition()
        {
            Transform t = transform;
            Vector3 pos = t.position;
            
            moveVector = velocity * Time.deltaTime;

            if (Physics.BoxCast(pos, halfExtends, moveVector.normalized, out RaycastHit hit, t.rotation, moveVector.magnitude, hitMask))
            {
                pos = hit.point;
                OnHit(hit.collider);
            }
            else
            {
                pos += moveVector;
            }
            
            t.position = pos;

            velocity -= t.forward * (decel * Time.deltaTime);
        }

        private void OnHit(Collider other)
        {
            if (!canHitSpawner && other.gameObject == spawner)
            {
                return;
            }
            
            Debug.Log($"{name} hit {other.name}");

            if (other.TryGetComponent(out Health health))
            {
                health.DealDamage(damage);
            }
            
            Destroy(gameObject);
        }

        private IEnumerator SpawnerHitDelay()
        {
            canHitSpawner = false;
            yield return new WaitForSeconds(spawnerHitDelay);
            canHitSpawner = true;
        }

        private void OnDrawGizmos()
        {
            Transform transform1 = transform;
            
            float distance = debugDistance;
            Vector3 forward = transform1.forward;
            
            if (Application.isPlaying)
            {
                distance = moveVector.magnitude;
                forward = moveVector.normalized;
            }
            
            Vector3 position = transform1.position;
            Vector3 origin = position + forward * distance;
            Vector3 right = transform1.right;
            Vector3 up = transform1.up;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, origin);

            Vector3 c1 = origin + forward * halfExtends.z + right * halfExtends.x + up * halfExtends.y;
            Vector3 c2 = origin + forward * halfExtends.z + right * halfExtends.x - up * halfExtends.y;
            Vector3 c3 = origin + forward * halfExtends.z - right * halfExtends.x + up * halfExtends.y;
            Vector3 c4 = origin + forward * halfExtends.z - right * halfExtends.x - up * halfExtends.y;
            
            Vector3 c5 = origin - forward * halfExtends.z + right * halfExtends.x + up * halfExtends.y;
            Vector3 c6 = origin - forward * halfExtends.z + right * halfExtends.x - up * halfExtends.y;
            Vector3 c7 = origin - forward * halfExtends.z - right * halfExtends.x + up * halfExtends.y;
            Vector3 c8 = origin - forward * halfExtends.z - right * halfExtends.x - up * halfExtends.y;

            Gizmos.DrawLine(c1, c2);
            Gizmos.DrawLine(c2, c4);
            Gizmos.DrawLine(c4, c3);
            Gizmos.DrawLine(c3, c1);
            
            Gizmos.DrawLine(c5, c6);
            Gizmos.DrawLine(c6, c8);
            Gizmos.DrawLine(c8, c7);
            Gizmos.DrawLine(c7, c5);
            
            Gizmos.DrawLine(c1, c5);
            Gizmos.DrawLine(c2, c6);
            Gizmos.DrawLine(c3, c7);
            Gizmos.DrawLine(c4, c8);
        }
    }
}
