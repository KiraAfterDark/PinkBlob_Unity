using System;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace PinkBlob.Gameplay.Ai.StateMachine.States.MoveStates
{
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(CharacterController))]
    public class WanderMoveState : MoveState
    {
        public override string StateName() => "Wander Move State";

        private enum WanderPosition
        {
            Origin,
            Self
        }

        [Title("Wander Properties")]

        [SerializeField]
        private WanderPosition wanderPosition = WanderPosition.Origin;

        private Vector3 wanderAround;
        
        [Min(0)]
        [SerializeField]
        private float wanderRadius = 2f;

        [Min(0)]
        [SerializeField]
        private float minimumWanderDistance = 1f;

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
        
        // navigation
        private Seeker seeker;
        private Path path;
        private bool reachedEndOfPath;
        private CharacterController characterController;
        private int currentWaypoint = 0;
        private float distanceToWaypoint;
        private bool hasPath = false;
        
        private Vector3 startPosition;
        

        private void Awake()
        {
            seeker = GetComponent<Seeker>();
            characterController = GetComponent<CharacterController>();

            startPosition = transform.position;
            
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
                                                          Name = "Start",
                                                          
                                                          Enter = EnterStart,
                                                          Update = UpdateStart
                                                      }
                            },
                            {
                                SubStateValues.Pathing, new SubState
                                                      {
                                                          Id = SubStateValues.Pathing,
                                                          Name = "Pathing",
                                                          
                                                          Enter = EnterPathing,
                                                      }
                            },
                            {
                                SubStateValues.Wander, new SubState
                                                       {
                                                           Id = SubStateValues.Wander,
                                                           Name = "Wander",
                                                           
                                                           Enter = EnterWander,
                                                           Update = UpdateWander
                                                       }
                            },
                            {
                                SubStateValues.Cooldown, new SubState
                                                       {
                                                           Id = SubStateValues.Cooldown,
                                                           Name = "Cooldown",
                                                           
                                                           Enter = EnterCooldown,
                                                           Update = UpdateCooldown
                                                       }
                            },
                            {
                                SubStateValues.Finish, new SubState
                                                       {
                                                           Id = SubStateValues.Finish,
                                                           Name = "Finish",
                                                           
                                                           Enter = EnterFinish,
                                                       }
                            }
                        };
        }
        
        protected override void EnterMove()
        {
            canTransitionToSelf = false;
            
            SetSubState(SubStateValues.Start);
        }

        protected override void UpdateMove()
        {
            CurrentSubState?.Update?.Invoke();
        }

        protected override void ExitMove()
        {
            
        }
        
        #region Start

        private void EnterStart()
        {
            timer = startTime;

            switch (wanderPosition)
            {
                case WanderPosition.Origin:
                    wanderAround = startPosition;
                    break;
                case WanderPosition.Self:
                    wanderAround = transform.position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateStart()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                SetSubState(SubStateValues.Pathing);
            }
        }
        
        #endregion
        
        #region Pathing

        private void EnterPathing()
        {
            Vector2 wander = Random.insideUnitCircle * wanderRadius;
            if (wander.magnitude < minimumWanderDistance)
            {
                wander = wander.normalized * minimumWanderDistance;
            }
            Vector3 pos = wanderAround + new Vector3(wander.x, 0, wander.y);

            hasPath = false;
            seeker.StartPath(transform.position, pos, OnPathCreated);
        }

        private void OnPathCreated(Path p)
        {
            if (!p.error)
            {
                path = p;
                hasPath = true;
                SetSubState(SubStateValues.Wander);
            }
        }
        
        #endregion
        
        #region Wander

        private void EnterWander()
        {
            currentWaypoint = 0;
        }

        private void UpdateWander()
        {
            while (true)
            {
                distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < reachDestinationDistance) 
                {
                    if (currentWaypoint + 1 < path.vectorPath.Count) 
                    {
                        currentWaypoint++;
                    } 
                    else
                    {
                        SetSubState(SubStateValues.Cooldown);
                        break;
                    }
                } 
                else
                {
                    break;
                }
            }
            
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            Vector3 velocity = dir * movementSpeed;
            characterController.SimpleMove(velocity);

            UpdateRotation(velocity.normalized);
        }
        
        private void UpdateRotation(Vector3 aimDirection)
        {
            Vector3 forward = aimDirection;
            forward.y = 0;
            forward.Normalize();

            transform.forward = Vector3.RotateTowards(transform.forward, forward, rotationSpeed * Time.deltaTime, 0);
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
            public const int Pathing = 1;
            public const int Wander = 2;
            public const int Cooldown = 3;
            public const int Finish = 4;
        }

        public override void PrintDebug()
        {
            EditorGUILayout.LabelField($"Current Sub State: {CurrentSubState.Name}");

            if (hasPath)
            {
                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField($"Number of waypoints: {path.vectorPath.Count}");
                EditorGUILayout.LabelField($"Current waypoint: {currentWaypoint}");
                EditorGUILayout.LabelField($"Distance to waypoint: {distanceToWaypoint}");

                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField($"Start Position: {path.vectorPath[0]}");
                EditorGUILayout.LabelField($"End Position: {path.vectorPath[^1]}");

                EditorGUILayout.LabelField($"Position: {transform.position}");
                EditorGUILayout.LabelField($"Distance to end: {Vector3.Distance(transform.position, path.vectorPath[^1])}");
            }
        }

        private void OnDrawGizmos()
        {
            Transform t = transform;
            Vector3 position = t.position;
            Vector3 origin = wanderPosition switch
                             {
                                 WanderPosition.Origin => Application.isPlaying ? startPosition : position,
                                 WanderPosition.Self => position,
                                 _ => throw new ArgumentOutOfRangeException()
                             };

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(origin, wanderRadius);
        }
    }
}
