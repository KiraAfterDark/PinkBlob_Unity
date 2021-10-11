using PinkBlob.Gameplay.Player;
using PinkBlob.Localization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace PinkBlob.Ui.GameplayHud
{
    [RequireComponent(typeof(UIDocument))]
    public class AbilityHud : MonoBehaviour
    {
        private UIDocument document;

        private Label abilityName;

        private PlayerController player;

        public void Init(PlayerController player)
        {
            this.player = player;
            
            SetListeners();
        }

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            
            VisualElement root = document.rootVisualElement;
            abilityName = root.Q<Label>("ability_name");
        }

        private void OnEnable()
        {
            if (player)
            {
                SetListeners();
            }
        }

        private void OnDisable()
        {
            if (player)
            {
                UnsetListeners();
            }
        }

        private void SetListeners()
        {
            player.OnNewAbility += OnNewAbility;
        }

        private void UnsetListeners()
        {
            player.OnNewAbility -= OnNewAbility;
        }

        private void OnNewAbility()
        {
            abilityName.text = Loc.Instance.Dictionary.Abilities.GetEntry(player.Ability.Properties.NameKey, Loc.Instance.Language);
        }
    }
}
