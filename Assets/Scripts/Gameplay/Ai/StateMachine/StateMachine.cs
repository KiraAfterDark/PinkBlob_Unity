using System.Collections.Generic;
using UnityEngine;

namespace PinkBlob.Gameplay.Ai.StateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        public IState CurrentState => currentState;
        private IState currentState;

        public List<Transition> Transitions => transitions;
        private readonly List<Transition> transitions = new List<Transition>();

        private void Awake()
        {
            InitStateMachine();
        }

        protected abstract void InitStateMachine();

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnStart() { }

        private void FixedUpdate()
        {
            currentState?.UpdateState();

            CheckTransitions();
        }

        private void CheckTransitions()
        {
            foreach (Transition transition in transitions)
            {
                if (transition.FromState == currentState && transition.CanTransition())
                {
                    SetState(transition.ToState);
                    break;
                }
            }
        }

        protected void SetState(IState state)
        {
            currentState?.ExitState();
            currentState = state;
            currentState?.EnterState();
        }

        protected Transition From(IState state)
        {
            var transition = new Transition();
            transition.From(state);
            transitions.Add(transition);

            return transition;
        }

        public abstract void PrintDebug();
    }
}
