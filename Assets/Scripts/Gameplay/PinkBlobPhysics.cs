using UnityEngine;

namespace PinkBlob.Gameplay
{
    [CreateAssetMenu(fileName = "New Physics Config", menuName = "Physics Config")]
    public class PinkBlobPhysics : ScriptableObject
    {
        [SerializeField]
        private float gravity = -9.81f;
        
        public float Gravity => gravity;

        [Min(0)]
        [SerializeField]
        private float groundFrictionCoef = 50f;

        public float GroundFrictionCoef => groundFrictionCoef;

        [Min(0)]
        [SerializeField]
        private float airFrictionCoef = 30f;

        public float AirFrictionCoef => airFrictionCoef;

        [SerializeField]
        private LayerMask groundMask = default;

        public LayerMask GroundMask => groundMask;
    }
}
