using System.Collections.Generic;
using PinkBlob.Gameplay.Ability.Properties;
using PinkBlob.Gameplay.Player;
using PinkBlob.Gameplay.Suck;
using UnityEditor;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability
{
    public class NormalAbility : PlayerAbility
    {
        private readonly Camera camera;
        
        private bool isSucking = false;
        
        private float turnSmoothVelocity;

        private NormalAbilityProperties NormalProperties => Properties as NormalAbilityProperties;

        private readonly Collider[] suckOverlap = new Collider[20];
        private int suckOverlapSize = 0;

        private bool hasObject = false;
        private SuckObject suckObject;
        private AbilityType suckType;
        private SuckObjectProperties suckObjectProperties;

        private readonly List<ISuckable> sucking = new List<ISuckable>();

        public override float RotationSpeedMod
        {
            get
            {
                if (isSucking)
                {
                    return NormalProperties.SuckRotationSpeedMod;
                }
                
                if (hasObject)
                {
                    return NormalProperties.InhaledRotationSpeedMod;
                }

                return 1;
            }
        }

        public override float MaxInputSpeedMod 
        {
            get
            {
                if (isSucking)
                {
                    return NormalProperties.SuckAccelMod;
                }
                
                if (hasObject)
                {
                    return NormalProperties.InhaledAccelMod;
                }

                return 1;
            }
        }
        
        private readonly GameObject suckFx;
        
        public NormalAbility(PlayerController player, Animator animator) : base(player, animator)
        {
            Properties = GameplayController.Instance.AbilityPropertyGroup.NormalAbilityProperties;

            camera = Camera.main;

            suckFx = Object.Instantiate(NormalProperties.SuckFx, player.SuckLocation);
            suckFx.SetActive(false);
        }

        protected override void ExitAbility()
        {
            Object.Destroy(suckFx);
        }

        public override void FixedUpdate()
        {
            if (isSucking)
            {
                CheckSuckable();
                UpdateSucking();
            }
        }

        private void CheckSuckable()
        {
            Transform transform = Player.transform;
            Vector3 origin = Player.SuckLocation.position;
            Vector3 areaCenter = origin
                               + transform.forward * (NormalProperties.SuckDistance / 2f);
                
            suckOverlapSize = Physics.OverlapBoxNonAlloc(areaCenter,
                                                         new Vector3(NormalProperties.SuckWidth, NormalProperties.SuckWidth,
                                                                     NormalProperties.SuckDistance),
                                                         suckOverlap, transform.rotation, NormalProperties.SuckMask);

            var currentlySucking = new List<ISuckable>(sucking);
            for (var i = 0; i < suckOverlapSize; i++)
            {
                if (suckOverlap[i].TryGetComponent(out ISuckable suckable))
                {
                    if (currentlySucking.Contains(suckable))
                    {
                        currentlySucking.Remove(suckable);
                    }
                    else
                    {
                        sucking.Add(suckable);
                        suckable.EnterSucking();

                        suckable.CompleteSucking += ObjectInhaled;
                    }
                }
            }
            
            // what's left is what's no longer in the sucking area
            foreach (ISuckable notSucking in currentlySucking)
            {
                notSucking.ExitSucking();
                sucking.Remove(notSucking);
            }
        }

        private void UpdateSucking()
        {
            foreach (ISuckable suckable in sucking)
            {
                suckable.UpdateSucking(Player.SuckLocation.position);
            }
        }

        private void ObjectInhaled(AbilityType abilityType)
        {
            hasObject = true;
            FlyingLock = true;
            isSucking = false;
            
            Animator.SetBool(NormalProperties.InhaledParam, true);

            suckObject = NormalProperties.SuckObject;
            suckType = abilityType;
            suckFx.SetActive(false);
            
            sucking.Clear();
        }

        public override void OnStartAction()
        {
            if (!hasObject)
            {
                isSucking = true;

                suckFx.SetActive(true);
            }
        }

        public override void OnCancelAction()
        {
            if (!hasObject)
            {
                isSucking = false;
                suckFx.SetActive(false);
                sucking.Clear();
            }
        }

        public override void OnPerformedAction()
        {
            if (hasObject)
            {
                hasObject = false;
                FlyingLock = false;

                Animator.SetBool(NormalProperties.InhaledParam, false);

                SuckObject suckObj = Object.Instantiate(suckObject, Player.SuckLocation.position, Player.SuckLocation.rotation);
                suckObj.Init(suckType);
            }
        }

        public override void OnPerformedCrouch()
        {
            if (hasObject)
            {
                Swallow();
            }
        }

        private void Swallow()
        {
            hasObject = false;
            Animator.SetBool(NormalProperties.InhaledParam, false);
            Animator.SetTrigger(NormalProperties.SwallowedTrigger);
            Player.SetAbility(suckType);
        }

        public override void PrintDebugWindow()
        {
            GUILayout.Label("Normal Ability", EditorStyles.boldLabel);
            GUILayout.Label($"Input Lock: {MovementInputLock}");
            GUILayout.Label($"Accel Mod: {MaxInputSpeedMod}");
            GUILayout.Label($"Rotation Speed Mod: {RotationSpeedMod}");

            GUILayout.Space(5);
            
            GUILayout.Label($"Has Object: {hasObject}");
            if (hasObject)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Label($"Type: {suckObject.AbilityType}");

                EditorGUI.indentLevel--;
            }
        }

        public override void OnDrawGizmos()
        {
            if (isSucking)
            {
                bool hasSuck = suckOverlapSize > 0;
                Gizmos.color = hasSuck ? Color.green : Color.blue;

                Vector3 origin = Player.SuckLocation.position;
                Transform transform = Player.transform;

                Vector3 forward = transform.forward;
                Vector3 up = transform.up;
                Vector3 right = transform.right;

                Vector3 c1 = origin + right * NormalProperties.SuckWidth + up * NormalProperties.SuckWidth;
                Vector3 c2 = origin + -right * NormalProperties.SuckWidth + up * NormalProperties.SuckWidth;
                Vector3 c3 = origin + -right * NormalProperties.SuckWidth + -up * NormalProperties.SuckWidth;
                Vector3 c4 = origin + right * NormalProperties.SuckWidth + -up * NormalProperties.SuckWidth;

                Vector3 c5 = c1 + forward * NormalProperties.SuckDistance;
                Vector3 c6 = c2 + forward * NormalProperties.SuckDistance;
                Vector3 c7 = c3 + forward * NormalProperties.SuckDistance;
                Vector3 c8 = c4 + forward * NormalProperties.SuckDistance;

                Gizmos.DrawLine(c1, c2);
                Gizmos.DrawLine(c2, c3);
                Gizmos.DrawLine(c3, c4);
                Gizmos.DrawLine(c4, c1);
                
                Gizmos.DrawLine(c5, c6);
                Gizmos.DrawLine(c6, c7);
                Gizmos.DrawLine(c7, c8);
                Gizmos.DrawLine(c8, c5);
                
                Gizmos.DrawLine(c1, c5);
                Gizmos.DrawLine(c2, c6);
                Gizmos.DrawLine(c3, c7);
                Gizmos.DrawLine(c4, c8);
            }
        }
    }
}
