using UnityEngine;

namespace PinkBlob.Gameplay
{
    public class StageController : MonoBehaviour
    {
        private void Awake()
        {
            GameplayEvents.StageLoaded(this);
        }
    }
}
