namespace PinkBlob.Gameplay.Ai.StateMachine.States
{
    public interface IState
    {

        string StateName();

        void EnterState();
        
        void UpdateState();
        
        void ExitState();

        bool CanTransitionIn();
        
        bool CanTransitionOut();

        bool CanTransitionToSelf();

        string Print();
    }
}
