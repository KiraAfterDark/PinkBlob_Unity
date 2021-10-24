using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability.Properties
{
    public class AbilityProperties : SerializedScriptableObject
    {
        [Title("Information")]

        [SerializeField]
        private string nameKey = "";

        public string NameKey => nameKey;

        [SerializeField]
        private Sprite icon;
        
        public Sprite Icon => icon;
        
        [Title("Visuals")]

        [SerializeField]
        private Material material = default;

        public Material Material => material;
    }
}
