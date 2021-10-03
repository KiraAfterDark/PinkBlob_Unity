using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PinkBlob
{
    public static class SceneHandler
    {
        private const string StageSuffix = "_STG";

        public static bool IsStageLoaded()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name.Contains(StageSuffix))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
