using UnityEngine;

namespace PinkBlob.Localization
{
    [CreateAssetMenu(fileName = "New Loc Dictionary", menuName = "Loc/Dictionary")]
    public class LocDictionary : ScriptableObject
    {
        [SerializeField]
        private LocCategory abilities;

        public LocCategory Abilities => abilities;
    }
}
