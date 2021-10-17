using System;
using PinkBlob.Gameplay.Player;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability
{
    public static class PlayerAbilityFactory
    {
        public static PlayerAbility GetAbility(this PlayerController player, Animator animator, AbilityType type)
        {
            return type switch
                   {
                       AbilityType.Normal => new NormalAbility(player, animator),
                       AbilityType.Fire => new FireAbility(player, animator),
                       AbilityType.Sword => throw new ArgumentOutOfRangeException(nameof(type), type, null),
                       AbilityType.Cutter => new CutterAbility(player, animator),
                       _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                   };
        }
    }
}
