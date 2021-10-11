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
        private SuckObjectProperties suckObjectProperties;

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

        public override Material Material => NormalProperties.Material;

        public NormalAbility(PlayerController playerController) : base(playerController)
        {
            Properties = GameplayController.Instance.AbilityPropertyGroup.NormalAbilityProperties;

            camera = Camera.main;

            suckFx = Object.Instantiate(NormalProperties.SuckFx, playerController.SuckLocation);
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
                Transform transform = PlayerController.transform;
                Vector3 origin = PlayerController.SuckLocation.position;
                Vector3 areaCenter = origin
                                   + transform.forward * (NormalProperties.SuckDistance / 2f);
                
                suckOverlapSize = Physics.OverlapBoxNonAlloc(areaCenter,
                                                             new Vector3(NormalProperties.SuckWidth, NormalProperties.SuckWidth,
                                                                         NormalProperties.SuckDistance),
                                                             suckOverlap, transform.rotation, NormalProperties.SuckMask);

                for (var i = 0; i < suckOverlapSize; i++)
                {
                    if (suckOverlap[i].TryGetComponent(out Suckable suck))
                    {
                        suck.Suck(PlayerController.SuckLocation.position, ObjectInhaled);
                    }
                }
            }
        }

        private void ObjectInhaled(SuckObject suckObject)
        {
            hasObject = true;
            FlyingLock = true;
            isSucking = false;

            this.suckObject = suckObject;
            suckFx.SetActive(false);
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
            }
        }

        public override void OnPerformedAction()
        {
            if (hasObject)
            {
                hasObject = false;
                FlyingLock = false;

                Object.Instantiate(suckObject, PlayerController.SuckLocation.position, PlayerController.SuckLocation.rotation);
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
            PlayerController.SetAbility(suckObject.AbilityType);
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

                Vector3 origin = PlayerController.SuckLocation.position;
                Transform transform = PlayerController.transform;

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
