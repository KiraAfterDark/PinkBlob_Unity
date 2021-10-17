using System;
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

        public List<Func<bool>> ExtraCheck => extraChecks;
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
            Interrupt = true;
            return this;
        }

        public bool CanTransition()
        {
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
