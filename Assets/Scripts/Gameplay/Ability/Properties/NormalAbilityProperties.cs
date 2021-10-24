using System.Collections.Generic;
using PinkBlob.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability.Properties
{
    [CreateAssetMenu(menuName = "Ability/Normal Ability Properties", fileName = "New Normal Ability Properties")]
    public class NormalAbilityProperties : AbilityProperties
    {
        [Title("Suck Attack")]
        
        [Min(0)]
        [SerializeField]
        private float suckDistance = 2f;

        public float SuckDistance => suckDistance;
        
        [Min(0)]
        [SerializeField]
        private float suckWidth = 1f;

        public float SuckWidth => suckWidth;

        [Min(0)]
        [SerializeField]
        private float suckRotationSpeedMod = 0.25f;

        public float SuckRotationSpeedMod => suckRotationSpeedMod;

        [Min(0)]
        [SerializeField]
        private float suckAccelMod = 0.25f;

        public float SuckAccelMod => suckAccelMod;

        [SerializeField]
        private LayerMask suckMask = default;

        public LayerMask SuckMask => suckMask;

        [SerializeField]
        private GameObject suckFx = default;

        public GameObject SuckFx => suckFx;

        [SerializeField]
        private SuckObject suckObject;
        
        public SuckObject SuckObject => suckObject;
        
        [Title("Inhaled")]

        [Min(0)]
        [SerializeField]
        private float inhaledRotationSpeedMod = 0.25f;

        public float InhaledRotationSpeedMod => inhaledRotationSpeedMod;

        [Min(0)]
        [SerializeField]
        private float inhaledAccelMod = 0.25f;

        public float InhaledAccelMod => inhaledAccelMod;

        [SerializeField]
        private string inhaledParam = "Inhaled";

        public string InhaledParam => inhaledParam;

        [SerializeField]
        private string swallowedTrigger = "Swallowed";

        public string SwallowedTrigger => swallowedTrigger;
    }
}
