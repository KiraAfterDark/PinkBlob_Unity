using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace PinkBlob
{
    public class WanderMoveState : MoveState
    {
        public override string StateName() => "Wander Move State";

        [Title("Wander Properties")]

        [SerializeField]
        private AgentType agentType;
        
        private NavMeshAgent agent;

        [Min(0)]
        [SerializeField]
        private float wanderRadius = 2f;

        [Min(0)]
        [SerializeField]
        private float reachDestinationDistance = 0.25f;

        [Title("Timings")]

        [Min(0)]
        [SerializeField]
        private float startTime = 0;

        [Min(0)]
        [SerializeField]
        private float cooldownTimer = 0;

        private float timer = 0;

        public override bool CanTransitionToSelf() => canTransitionToSelf;

        private bool canTransitionToSelf = false;

        private void Awake()
        {
            if (!TryGetComponent(out agent))
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
            }

            agent.agentTypeID = AgentUtil.AgentTypeToId(agentType);

            SetupSubStateMap();
        }

        private void SetupSubStateMap()
        {
            subStates = new Dictionary<int, SubState>
                        {
                            {
                                SubStateValues.Start, new SubState
                                                      {
                                                          Id = SubStateValues.Start,
                                                          
                                                          Enter = EnterStart,
                                                          Update = UpdateStart
                                                      }
                            },
                            {
                                SubStateValues.Wander, new SubState
                                                       {
                                                           Id = SubStateValues.Wander,
                                                           
                                                           Enter = EnterWander,
                                                           Update = UpdateWander
                                                       }
                            },
                            {
                                SubStateValues.Cooldown, new SubState
                                                       {
                                                           Id = SubStateValues.Cooldown,
                                                           
                                                           Enter = EnterCooldown,
                                                           Update = UpdateCooldown
                                                       }
                            },
                            {
                                SubStateValues.Finish, new SubState
                                                       {
                                                           Id = SubStateValues.Finish,
                                                           
                                                           Enter = EnterFinish,
                                                       }
                            }
                        };
        }
        
        protected override void EnterMove()
        {
            agent.enabled = true;
            agent.speed = movementSpeed;
            agent.angularSpeed = rotationSpeed;

            canTransitionToSelf = false;
            
            SetSubState(SubStateValues.Start);
        }

        protected override void UpdateMove()
        {
            CurrentSubState?.Update?.Invoke();
        }

        protected override void ExitMove()
        {
            agent.enabled = false;
        }
        
        #region Start

        private void EnterStart()
        {
            Debug.Log("Enter Start");
            
            timer = startTime;
        }

        private void UpdateStart()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                SetSubState(SubStateValues.Wander);
            }
        }
        
        #endregion
        
        #region Wander

        private void EnterWander()
        {
            Debug.Log("Enter Wander");
            
            Vector2 wander = Random.insideUnitCircle * wanderRadius;
            Vector3 pos = transform.position + new Vector3(wander.x, 0, wander.y);

            if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                pos = hit.position;
            }

            Debug.Log(pos);

            agent.SetDestination(pos);
            agent.isStopped = false;
        }

        private void UpdateWander()
        {
            if (agent.remainingDistance < reachDestinationDistance)
            {
                SetSubState(SubStateValues.Cooldown);
            }
        }
        
        #endregion
        
        #region Cooldown
        
        private void EnterCooldown()
        {
            timer = cooldownTimer;
        }

        private void UpdateCooldown()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                SetSubState(SubStateValues.Finish);
            }
        }
        
        #endregion
        
        #region Finish

        private void EnterFinish()
        {
            canTransitionToSelf = true;
        }
        
        #endregion

        public override bool CanTransitionOut()
        {
            return true;
        }

        private static class SubStateValues
        {
            public const int Start = 0;
            public const int Wander = 1;
            public const int Cooldown = 2;
            public const int Finish = 3;
        }
    }
}
