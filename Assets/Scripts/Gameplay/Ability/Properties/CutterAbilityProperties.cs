using PinkBlob.Gameplay.Ability.Objects;
using PinkBlob.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability.Properties
{
    [CreateAssetMenu(menuName = "Ability/Cutter Ability Properties", fileName = "New Cutter Ability Properties")]

    public class CutterAbilityProperties : AbilityProperties
    {
        [Title("Cutter Properties")]

        [SerializeField]
        private CutterObject prefab;

        public CutterObject Prefab => prefab;

        [Min(0)]
        [SerializeField]
        private int damage = 5;

        public int Damage => damage;

        [Min(1)]
        [SerializeField]
        private int maxCutters = 3;

        public int MaxCutters => maxCutters;
    }
}
