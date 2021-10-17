using PinkBlob.Gameplay.Ai.StateMachine.States.StunStates;
using PinkBlob.Gameplay.Enemy;
using PinkBlob.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PinkBlob.Gameplay.Ai.StateMachine.EnemyStateMachines
{
    public class WanderEnemyStateMachine : StateMachine
    {
        private PlayerController player;
        
        [Title("Enemy Properties")]

        [SerializeField]
        private LineOfSight lineOfSight;

        [Title("Wander Enemy States")]

        [SerializeField]
        private MoveState wander;

        [SerializeField]
        private MoveState track;

        [SerializeField]
        private StunState inhaleStun;

        [Tooltip("When player is in this range, enemy will track player")]
        [Min(0)]
        [SerializeField]
        private float trackingRange = 10f;

        protected override void InitStateMachine()
        {
            From(wander).To(wander);
            From(wander).To(track).When(() => CheckDistanceToPlayer(trackingRange)
                                           && lineOfSight.HasLineOfSight(player));

            From(track).To(track);
            From(track).To(wander).When(() => !CheckDistanceToPlayer(trackingRange)
                                           || !lineOfSight.HasLineOfSight(player));

            From(track).To(inhaleStun).IsInterrupt().When(() => EnemyController.IsSucking());
            From(inhaleStun).To(wander).When(() => !EnemyController.IsSucking());
            
        }

        protected override void OnStart()
        {
            player = GameplayController.Instance.PlayerController;
            SetState(wander);
        }

        private bool CheckDistanceToPlayer(float range)
        {
            if (!player)
            {
                Debug.Log("No player");
                return false;
            }

            return Vector3.Distance(transform.position, GameplayController.Instance.PlayerController.transform.position) < range;
        }

        public override void PrintDebug()
        {
            Vector3 playerPos = player.transform.position;
            
            EditorGUILayout.LabelField($"Distance to player: {Vector3.Distance(transform.position, playerPos)}");

            if (lineOfSight)
            {
                lineOfSight.PrintDebug(player);
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = lineOfSight.HasLineOfSight(player) ? Color.green : Color.red;
                Gizmos.DrawLine(player.transform.position, transform.position);
            }
        }
    }
}
