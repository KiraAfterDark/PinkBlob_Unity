using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Localization
{
    [CreateAssetMenu(fileName = "New Loc Category", menuName = "Loc/Category")]
    public class LocCategory : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<string, Dictionary<LanguageCode, string>> entries;

        public string GetEntry(string stringKey, LanguageCode languageCode)
        {
            return entries[stringKey][languageCode];
        }
    }
}
