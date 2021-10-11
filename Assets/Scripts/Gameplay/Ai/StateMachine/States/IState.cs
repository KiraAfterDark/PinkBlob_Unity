using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PinkBlob
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
    }
}
