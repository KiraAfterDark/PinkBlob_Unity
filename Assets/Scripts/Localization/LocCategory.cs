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
            if (entries.ContainsKey(stringKey))
            {
                if (entries[stringKey].ContainsKey(languageCode))
                {
                    return entries[stringKey][languageCode];
                }
                
                Debug.LogWarning($"String Key {stringKey} is missing language {languageCode}.", this);
                return stringKey;
            }
            
            Debug.LogWarning($"String Key {stringKey} is missing in LocCategory {name}.", this);
            return stringKey;
        }
    }
}
