using PinkBlob.Gameplay.Ai.StateMachine;
using UnityEngine;

namespace PinkBlob.Gameplay.Enemy
{
    [RequireComponent(typeof(StateMachine))]
    public class EnemyController : MonoBehaviour
    {
        public StateMachine StateMachine => stateMachine;
        private StateMachine stateMachine;

        private void Awake()
        {
            stateMachine = GetComponent<StateMachine>();
        }

        private void Start()
        {
            GameplayController.Instance.SpawnEnemy(this);
        }
    }
}
