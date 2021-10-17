using UnityEngine;

namespace PinkBlob.Gameplay.Ai.StateMachine.States.StunStates
{
    // Literally does nothing...
    public class StunState : MonoBehaviour, IState
    {
        public string StateName() => "Basic Stun State";

        public void EnterState() { }

        public void UpdateState() { }

        public void ExitState() { }
        public bool CanTransitionIn() => true;
        public bool CanTransitionOut() => true;
        public bool CanTransitionToSelf() => false;
        public void PrintDebug() { }
    }
}
