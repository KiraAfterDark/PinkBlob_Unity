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

        private Vector3 velocity = Vector3.zero;
        

        public CutterAbility(PlayerController player, Animator animator) : base(player, animator)
        {
            Properties = GameplayController.Instance.AbilityPropertyGroup.CutterAbilityProperties;
        }

        protected override void ExitAbility()
        {
            
        }

        public override void OnPerformedAction()
        {
            Transform t = Player.RangeOrigin;
            
            Vector3 origin = t.position;
            Vector3 forward = t.forward;

            CutterObject cutterObj = Object.Instantiate(CutterProperties.Prefab);

            Transform c = cutterObj.transform;
            c.position = origin;
            c.forward = forward;
            cutterObj.Init(Player.gameObject, Player.Velocity, CutterProperties.Damage);

        }

        public override void PrintDebugWindow()
        {
            base.PrintDebugWindow();
            GUILayout.Space(5);
            GUILayout.Label("Cutter Ability", EditorStyles.boldLabel);
        }
    }
}
