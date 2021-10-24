using System;
using System.Collections.Generic;
using PinkBlob.Gameplay.Ai.StateMachine.States;

namespace PinkBlob.Gameplay.Ai.StateMachine
{
    public class Transition
    {
        public IState FromState { get; private set; }
        public IState ToState { get; private set; }

        private bool interrupt;

        private readonly List<Func<bool>> extraChecks = new List<Func<bool>>();

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

        public Transition When(Func<bool> check)
        {
            extraChecks.Add(check);
            return this;
        }

        public Transition IsInterrupt()
        {
            interrupt = true;
            return this;
        }

        public bool CanTransition()
        {
            if (interrupt)
            {
                return ToState.CanTransitionIn() && ExtraChecks() && CheckSelfTransition();
            }
            
            return FromState.CanTransitionOut() && ToState.CanTransitionIn() && ExtraChecks() && CheckSelfTransition();
        }

        public bool ExtraChecks()
        {
            foreach (Func<bool> extraCheck in extraChecks)
            {
                if (!extraCheck.Invoke())
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckSelfTransition()
        {
            return FromState != ToState || FromState.CanTransitionToSelf();
        }
    }
}
