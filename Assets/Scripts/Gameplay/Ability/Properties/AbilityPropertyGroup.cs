using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability.Properties
{
    [CreateAssetMenu(fileName = "New Ability Property Group", menuName = "Ability/Ability Property Group", order = 0)]
    public class AbilityPropertyGroup : SerializedScriptableObject
    {
        public AbilityPropertyGroup(NormalAbilityProperties normalAbilityProperties, FireAbilityProperties fireAbilityProperties,
                                    CutterAbilityProperties cutterAbilityProperties)
        {
            NormalAbilityProperties = normalAbilityProperties;
            FireAbilityProperties = fireAbilityProperties;
            CutterAbilityProperties = cutterAbilityProperties;
        }

        [SerializeField]
        public NormalAbilityProperties NormalAbilityProperties { get; private set; }
        
        [SerializeField]
        public FireAbilityProperties FireAbilityProperties { get; private set; }

        [SerializeField]
        public CutterAbilityProperties CutterAbilityProperties { get; private set; }
    }
}
