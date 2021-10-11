using System;
using PinkBlob.Gameplay.Ability;
using UnityEngine;

namespace PinkBlob.Gameplay.Suck
{
    [Serializable]
    public class SuckObjectProperties
    {
        [SerializeField]
        private AbilityType type = AbilityType.Normal;

        [SerializeField]
        private GameObject suckObject = default;
    }
}
