using System;
using UnityEngine;

namespace PinkBlob.Gameplay
{
    public static class GameplayEvents
    {
        public static void StageLoaded(StageController stage)
        {
            OnStageLoaded?.Invoke(stage);
        }
        
        public static event Action<StageController> OnStageLoaded;
    }
}
