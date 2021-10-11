using PinkBlob.Gameplay.Ability.Properties;
using PinkBlob.Gameplay.Player;
using UnityEditor;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability
{
    public class FireAbility : PlayerAbility
    {
        private bool isFlamethrower = false;
        
        private float turnSmoothVelocity;

        private FireAbilityProperties FireProperties => Properties as FireAbilityProperties;

        private readonly Collider[] flamethrowerOverlap = new Collider[20];
        private int flamethrowerOverlapSize = 0;

        public override float RotationSpeedMod
        {
            get
            {
                if (isFlamethrower)
                {
                    return FireProperties.FlamethrowerRotationSpeedMod;
                }

                return 1;
            }
        }

        public override float MaxInputSpeedMod 
        {
            get
            {
                if (isFlamethrower)
                {
                    return FireProperties.FlamethrowerAccelMod;
                }

                return 1;
            }
        }
        
        private readonly GameObject flamethrowerFx;

        private float timer = 0;

        public override Material Material => FireProperties.Material;

        public FireAbility(PlayerController playerController) : base(playerController)
        {
            Properties = GameplayController.Instance.AbilityPropertyGroup.FireAbilityProperties;
            
            flamethrowerFx = Object.Instantiate(FireProperties.FlamethrowerFx, playerController.SuckLocation);
            flamethrowerFx.SetActive(false);

            timer = 0;
        }
        
        protected override void ExitAbility()
        {
            Object.Destroy(flamethrowerFx);
        }

        public override void FixedUpdate()
        {
            if (isFlamethrower)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    timer = FireProperties.FlamethrowerTime;

                    Transform transform = PlayerController.transform;
                    Vector3 origin = PlayerController.SuckLocation.position;

                    if (Physics.SphereCast(origin, FireProperties.FlamethrowerWidth, transform.forward, out RaycastHit hit,
                                           FireProperties.FlamethrowerDistance, FireProperties.FlamethrowerMask))
                    {
                        if (hit.collider.TryGetComponent(out Health health))
                        {
                            health.DealDamage(FireProperties.Damage);
                        }
                    }
                }
            }
        }

        public override void OnStartAction()
        {
            if (!isFlamethrower)
            {
                isFlamethrower = true;

                flamethrowerFx.SetActive(true);
            }
        }

        public override void OnCancelAction()
        {
            if (isFlamethrower)
            {
                isFlamethrower = false;
                flamethrowerFx.SetActive(false);
            }
        }

        public override void PrintDebugWindow()
        {
            GUILayout.Label("Fire Ability", EditorStyles.boldLabel);
            GUILayout.Label($"Input Lock: {MovementInputLock}");
            GUILayout.Label($"Accel Mod: {MaxInputSpeedMod}");
            GUILayout.Label($"Rotation Speed Mod: {RotationSpeedMod}");

            if (GUILayout.Button("Toggle Flamethrower"))
            {
                if (isFlamethrower)
                {
                    isFlamethrower = false;
                    flamethrowerFx.SetActive(false);
                }
                else
                {
                    isFlamethrower = true;
                    flamethrowerFx.SetActive(true);
                }
            }
        }

        public override void OnDrawGizmos()
        {
            if (isFlamethrower)
            {
                Gizmos.color = Color.red;

                Vector3 origin = PlayerController.SuckLocation.position;
                Transform transform = PlayerController.transform;

                Vector3 forward = transform.forward;
                Vector3 up = transform.up;
                Vector3 right = transform.right;

                Vector3 c1 = origin + right * FireProperties.FlamethrowerWidth + up * FireProperties.FlamethrowerWidth;
                Vector3 c2 = origin + -right * FireProperties.FlamethrowerWidth + up * FireProperties.FlamethrowerWidth;
                Vector3 c3 = origin + -right * FireProperties.FlamethrowerWidth + -up * FireProperties.FlamethrowerWidth;
                Vector3 c4 = origin + right * FireProperties.FlamethrowerWidth + -up * FireProperties.FlamethrowerWidth;

                Vector3 c5 = c1 + forward * FireProperties.FlamethrowerDistance;
                Vector3 c6 = c2 + forward * FireProperties.FlamethrowerDistance;
                Vector3 c7 = c3 + forward * FireProperties.FlamethrowerDistance;
                Vector3 c8 = c4 + forward * FireProperties.FlamethrowerDistance;

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
