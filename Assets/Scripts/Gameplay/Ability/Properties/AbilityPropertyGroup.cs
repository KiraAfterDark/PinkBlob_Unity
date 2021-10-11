using UnityEngine;

namespace PinkBlob.Gameplay.Ability.Properties
{
    [CreateAssetMenu(fileName = "New Ability Property Group", menuName = "Ability/Ability Property Group", order = 0)]
    public class AbilityPropertyGroup : ScriptableObject
    {
        [SerializeField]
        private NormalAbilityProperties normalAbilityProperties = default;

        public NormalAbilityProperties NormalAbilityProperties => normalAbilityProperties;
        
        [SerializeField]
        private FireAbilityProperties fireAbilityProperties = default;

        public FireAbilityProperties FireAbilityProperties => fireAbilityProperties;
    }
}
