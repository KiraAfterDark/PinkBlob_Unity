using System;
using PinkBlob.Gameplay.Ability;
using UnityEngine;

namespace PinkBlob.Gameplay.Suck
{
    public interface ISuckable
    {
        event Action<AbilityType> OnCompleteSucking;

        void EnterSucking();
        
        void UpdateSucking(Vector3 source);
        void ExitSucking();
    }
}
