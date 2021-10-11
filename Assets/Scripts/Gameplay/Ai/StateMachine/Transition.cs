using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PinkBlob
{
    public class Transition
    {
        public IState FromState { get; private set; }
        public IState ToState { get; private set; }
        
        public bool Interrupt { get; private set; }

        private bool extraCheck = true;

        public Transition From(IState state)
        {
            FromState = state;
            return this;
        }

        public Transition To(IState state)
        {
            ToState = state;
            return this;
        }

        public Transition When(bool check)
        {
            extraCheck &= check;
            return this;
        }

        public Transition IsInterrupt()
        {
            Interrupt = true;
            return this;
        }

        public bool CanTransition()
        {
            return FromState.CanTransitionOut() && ToState.CanTransitionIn() && extraCheck && CheckSelfTransition();
        }

        private bool CheckSelfTransition()
        {
            return FromState != ToState || FromState.CanTransitionToSelf();
        }
    }
}
