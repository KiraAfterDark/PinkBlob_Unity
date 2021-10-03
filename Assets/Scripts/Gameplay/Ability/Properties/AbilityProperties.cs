using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability.Properties
{
    public class AbilityProperties : ScriptableObject
    {
        [Title("Information")]

        [SerializeField]
        private string nameKey = "";

        public string NameKey => nameKey;
        
        [Title("Visuals")]

        [SerializeField]
        private Material material = default;

        public Material Material => material;
    }
}
