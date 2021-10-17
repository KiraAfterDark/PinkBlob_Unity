using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay
{
    public class StageController : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Transform playerSpawn;

        public Transform PlayerSpawn => playerSpawn;
        
        private void Awake()
        {
            GameplayEvents.StageLoaded(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(playerSpawn.position, 1);
        }
    }
}
