using System.Collections.Generic;
using PinkBlob.Gameplay.Ability.Objects;
using PinkBlob.Gameplay.Ability.Properties;
using PinkBlob.Gameplay.Player;
using UnityEditor;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability
{
    public class CutterAbility : PlayerAbility
    {
        private CutterAbilityProperties CutterProperties => Properties as CutterAbilityProperties;
        
        private readonly List<CutterObject> cutterObjects;

        private bool canThrow = true;
        private float timer = 0;
        
        public CutterAbility(PlayerController player, Animator animator) : base(player, animator)
        {
            Properties = GameplayController.Instance.AbilityPropertyGroup.CutterAbilityProperties;
            cutterObjects = new List<CutterObject>();
        }

        protected override void ExitAbility()
        {
            foreach (CutterObject cutterObject in cutterObjects)
            {
                cutterObject.CutterDeath -= OnCutterDeath;
            }

            cutterObjects.Clear();
        }

        public override void FixedUpdate()
        {
            if (!canThrow)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    canThrow = true;
                }
            }
        }

        public override void OnPerformedAction()
        {
            if (cutterObjects.Count > CutterProperties.MaxCutters || !canThrow)
            {
                return;
            }
            
            Transform t = Player.RangeOrigin;
            
            Vector3 origin = t.position;
            Vector3 forward = t.forward;

            CutterObject cutterObj = Object.Instantiate(CutterProperties.Prefab);

            Transform c = cutterObj.transform;
            c.position = origin;
            c.forward = forward;
            cutterObj.Init(Player.gameObject, Player.Velocity, CutterProperties.Damage);

            cutterObj.CutterDeath += OnCutterDeath;
            cutterObjects.Add(cutterObj);

            timer = CutterProperties.Cooldown;
            canThrow = false;
        }

        private void OnCutterDeath(CutterObject cutterObject)
        {
            cutterObject.CutterDeath -= OnCutterDeath;
            cutterObjects.Remove(cutterObject);
        }

        public override void PrintDebugWindow()
        {
            base.PrintDebugWindow();
            GUILayout.Space(5);
            GUILayout.Label("Cutter Ability", EditorStyles.boldLabel);
            GUILayout.Label($"Cutters active: {cutterObjects.Count}");
        }
    }
}
