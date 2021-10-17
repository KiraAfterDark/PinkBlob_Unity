using PinkBlob.Gameplay.Ability.Properties;
using PinkBlob.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace PinkBlob.Gameplay.Ability
{
    public abstract class PlayerAbility
    {
        public bool MovementInputLock { get; protected set; } = false;
        public bool FlyingLock { get; protected set; } = false;

        public virtual float RotationSpeedMod => 1;

        public virtual float MaxInputSpeedMod => 1f;

        protected readonly PlayerController Player;

        protected readonly Animator Animator;

        public Material Material => Properties.Material;

        public AbilityProperties Properties { get; protected set; }

        protected PlayerAbility(PlayerController player, Animator animator)
        {
            Player = player;
            Animator = animator;
            
            
        }

        protected abstract void ExitAbility();

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void OnStartAction() { }
        
        public virtual void OnCancelAction() { }
        
        public virtual void OnPerformedAction() { }

        public virtual void OnPerformedCrouch() { }

        public virtual void OnDrawGizmos() { }

        public virtual void PrintDebugWindow()
        {
            GUILayout.Label("Ability", EditorStyles.boldLabel);
            GUILayout.Label($"Input Lock: {MovementInputLock}");
            GUILayout.Label($"Accel Mod: {MaxInputSpeedMod}");
            GUILayout.Label($"Rotation Speed Mod: {RotationSpeedMod}");
        }
    }
}
