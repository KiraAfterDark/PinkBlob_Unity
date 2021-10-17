using System.Collections.Generic;
using Pathfinding;
using PinkBlob.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PinkBlob.Gameplay.Ai.StateMachine.States.MoveStates
{
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(Rigidbody))]
    public class TrackPlayerMoveState : MoveState
    {
        public override string StateName() => "Tracking Player Move State";

        [Title("Tracking Properties")]

        [Min(0)]
        [SerializeField]
        private float reachedDestinationDistance = 0.5f;

        [Min(0)]
        [SerializeField]
        private float updatePathTime;

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
        private Rigidbody rigidbody;
        private int currentWaypoint = 0;
        private float distanceToWaypoint;

        private void Awake()
        {
            seeker = GetComponent<Seeker>();
            rigidbody = GetComponent<Rigidbody>();

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
                                SubStateValues.Move, new SubState
                                                       {
                                                           Id = SubStateValues.Move,
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
            seeker.StartPath(transform.position, GameplayController.Instance.PlayerController.transform.position, OnPathCreated);
        }

        private void OnPathCreated(Path p)
        {
            if (!p.error)
            {
                path = p;
                SetSubState(SubStateValues.Move);
            }
        }
        
        #endregion
        
        #region Wander

        private void EnterWander()
        {
            timer = updatePathTime;
            currentWaypoint = 0;
        }

        private void UpdateWander()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                seeker.StartPath(transform.position, GameplayController.Instance.PlayerController.transform.position, OnPathCreated);
            }
            
            while (true)
            {
                distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < reachedDestinationDistance) 
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

            Vector3 position = transform.position;
            Vector3 dir = (path.vectorPath[currentWaypoint] - position).normalized;
            Vector3 velocity = dir * (movementSpeed * Time.deltaTime);
            rigidbody.MovePosition(position + velocity);

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
            public const int Move = 2;
            public const int Cooldown = 3;
            public const int Finish = 4;
        }

        public override void PrintDebug()
        {
            // EditorGUILayout.LabelField($"Current Sub State: {CurrentSubState.Name}");
            //
            // EditorGUILayout.Space(5);
            //
            // EditorGUILayout.LabelField($"Number of waypoints: {path.vectorPath.Count}");
            // EditorGUILayout.LabelField($"Current waypoint: {currentWaypoint}");
            // EditorGUILayout.LabelField($"Distance to waypoint: {distanceToWaypoint}");
            //
            // EditorGUILayout.Space(5);
            //
            // EditorGUILayout.LabelField($"Start Position: {path.vectorPath[0]}");
            // EditorGUILayout.LabelField($"End Position: {path.vectorPath[^1]}");
            // EditorGUILayout.LabelField($"Position: {transform.position}");
            // EditorGUILayout.LabelField($"Distance to end: {Vector3.Distance(transform.position, path.vectorPath[^1])}");
        }
    }
}
