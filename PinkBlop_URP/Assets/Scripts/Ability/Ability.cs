using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PinkBlob.Ability
{
    public abstract class Ability
    {
        public abstract void EquipAbility(PlayerController player);

        public abstract void UnequipAbility();

        public abstract void UpdateAbility();
    }
}
