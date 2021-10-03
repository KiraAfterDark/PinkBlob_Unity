using System;
using PinkBlob.Gameplay.Player;

namespace PinkBlob.Gameplay.Ability
{
    public static class PlayerAbilityFactory
    {
        public static PlayerAbility GetAbility(this PlayerController player, AbilityType type)
        {
            return type switch
                   {
                       AbilityType.Normal => new NormalAbility(player),
                       AbilityType.Fire => new FireAbility(player),
                       AbilityType.Sword => throw new ArgumentOutOfRangeException(nameof(type), type, null),
                       _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                   };
        }
    }
}
