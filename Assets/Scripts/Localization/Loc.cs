using Unity.VisualScripting;
using UnityEngine;

namespace PinkBlob.Localization
{
    public class Loc : MonoBehaviour
    {
        public static Loc Instance { get; private set; }
        
        [SerializeField]
        private LocDictionary dictionary;

        public LocDictionary Dictionary => dictionary;

        [SerializeField]
        private LanguageCode language;

        public LanguageCode Language => language;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(Instance);
            }

            Instance = this;
        }
    }
}
