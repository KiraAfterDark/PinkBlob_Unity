using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PinkBlob.Ability
{
    public class NormalAbility : Ability
    {
        private PlayerController player;
        
        public override void EquipAbility(PlayerController player)
        {
            this.player = player;
        }

        public override void UnequipAbility()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateAbility()
        {
            throw new System.NotImplementedException();
        }
    }
}
