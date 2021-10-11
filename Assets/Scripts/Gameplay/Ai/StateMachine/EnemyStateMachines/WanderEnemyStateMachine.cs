using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob
{
    public class WanderEnemyStateMachine : StateMachine
    {
        [Title("Wander Enemy States")]

        [SerializeField]
        private MoveState move;
        
        protected override void InitStateMachine()
        {
            From(move).To(move);
        }

        protected override void OnStart()
        {
            SetState(move);
        }
    }
}
