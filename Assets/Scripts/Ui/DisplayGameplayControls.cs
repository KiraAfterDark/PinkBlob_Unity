using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using PlayerInput = PinkBlob.Input.PlayerInput;

namespace PinkBlob.Ui
{
    [RequireComponent(typeof(UIDocument))]
    public class DisplayGameplayControls : MonoBehaviour
    {
        private Label moveLabel;
        private Label jumpLabel;
        private Label actionLabel;
        private Label copyLabel;
        private Label resetLabel;

        private PlayerInput playerInput;

        private void Awake()
        {
            playerInput = new PlayerInput();
            
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            moveLabel = root.Q<Label>("Move");
            jumpLabel = root.Q<Label>("Jump");
            actionLabel = root.Q<Label>("Action");
            copyLabel = root.Q<Label>("Copy");
            resetLabel = root.Q<Label>("Reset");

            var controls = "";
            foreach (InputBinding binding in playerInput.Gameplay.Movement.bindings)
            {
                controls += GetControlsFromBindings(binding);
            }
            moveLabel.text = "Move: " + controls;
            
            controls = "";
            foreach (InputBinding binding in playerInput.Gameplay.Jump.bindings)
            {
                controls += GetControlsFromBindings(binding);
            }
            jumpLabel.text = "Jump: " + controls;
            
            controls = "";
            foreach (InputBinding binding in playerInput.Gameplay.Action.bindings)
            {
                controls += GetControlsFromBindings(binding);
            }
            actionLabel.text = "Action: " + controls;
            
            controls = "";
            foreach (InputBinding binding in playerInput.Gameplay.Crouch.bindings)
            {
                controls += GetControlsFromBindings(binding);
            }
            copyLabel.text = "Crouch: " + controls;
            
            controls = "";
            foreach (InputBinding binding in playerInput.Gameplay.CancelAbility.bindings)
            {
                controls += GetControlsFromBindings(binding);
            }
            resetLabel.text = "Cancel Ability: " + controls;
        }

        private string GetControlsFromBindings(InputBinding binding)
        {
            var gamepad = "";
            var keyboard = ""; 
            var mouse = "";
            
            if (binding.path.Contains("<Gamepad>/"))
            {
                gamepad += binding.path.Split('/')[1] + " ";
            }
                
            if (binding.path.Contains("<Keyboard>/"))
            {
                keyboard += binding.path.Split('/')[1];
            }
            
            if (binding.path.Contains("<Mouse>/"))
            {
                mouse += binding.path.Split('/')[1];
            }

            return gamepad + " " + keyboard + " " + mouse;
        }
    }
}
