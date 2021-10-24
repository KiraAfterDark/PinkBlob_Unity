using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Ai.StateMachine.States.MoveStates
{
    public abstract class MoveState : MonoBehaviour, IState
    {
        public abstract string StateName();

        [Title("Move Properties")]

        [Min(0)]
        [SerializeField]
        protected float movementSpeed = 10f;

        [Min(0)]
        [SerializeField]
        protected float rotationSpeed = 10f;
        
        // sub state support
        protected Dictionary<int, SubState> subStates;
        protected SubState CurrentSubState { get; private set; }

        public void EnterState()
        {
            EnterMove();
        }

        protected abstract void EnterMove();

        public void UpdateState()
        {
            UpdateMove();
        }

        protected abstract void UpdateMove();

        public void ExitState()
        {
            ExitMove();
        }

        protected abstract void ExitMove();

        public virtual bool CanTransitionIn() => true;

        public virtual bool CanTransitionOut() => true;

        public virtual bool CanTransitionToSelf() => false;

        public abstract string Print();

        protected void SetSubState(int subStateId)
        {
            CurrentSubState?.Exit?.Invoke();
            CurrentSubState = subStates[subStateId];
            CurrentSubState?.Enter?.Invoke();
        }
    }
}
