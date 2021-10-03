using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability.Properties
{
    [CreateAssetMenu(menuName = "Ability/Fire Ability Properties", fileName = "New Fire Ability Properties")]
    public class FireAbilityProperties : AbilityProperties
    {
        [Title("Flamethrower Attack")]

        [Min(0)]
        [SerializeField]
        private int damage = 2;
        
        public int Damage => damage;
        
        [Min(0)]
        [SerializeField]
        private float flamethrowerDistance = 2f;

        public float FlamethrowerDistance => flamethrowerDistance;
        
        [Min(0)]
        [SerializeField]
        private float flamethrowerWidth = 1f;

        public float FlamethrowerWidth => flamethrowerWidth;

        [Min(0)]
        [SerializeField]
        private float flamethrowerRotationSpeedMod = 0.25f;

        public float FlamethrowerRotationSpeedMod => flamethrowerRotationSpeedMod;

        [Min(0)]
        [SerializeField]
        private float flamethrowerAccelMod = 0.25f;

        public float FlamethrowerAccelMod => flamethrowerAccelMod;

        [SerializeField]
        private LayerMask flamethrowerMask = default;

        public LayerMask FlamethrowerMask => flamethrowerMask;

        [Min(0)]
        [SerializeField]
        private float flamethrowerTime = 0.5f;

        public float FlamethrowerTime => flamethrowerTime;

        [SerializeField]
        private GameObject flamethrowerFx = default;

        public GameObject FlamethrowerFx => flamethrowerFx;
    }
}