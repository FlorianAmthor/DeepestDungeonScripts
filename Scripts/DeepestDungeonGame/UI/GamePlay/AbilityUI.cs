using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WatStudios.DeepestDungeon.UI.Gameplay
{
    [Serializable]
    public class AbilityUI : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private Image _abilityImage;
        [SerializeField] private Image _abilityCooldownImage;
        [SerializeField] private TextMeshProUGUI _abilityCooldownText;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private float _baseCooldown;
        #endregion

        #region Properties
        public int AbilityIndex { get; private set; }
        #endregion

        #region Internal Methods
        internal void Init(Sprite abilityImage, float baseCooldown, int abilityIndex)
        {
            _abilityImage.sprite = abilityImage;
            _baseCooldown = baseCooldown;
            _abilityCooldownImage.fillAmount = 0;
            _abilityCooldownText.text = _baseCooldown.ToString();

            _abilityCooldownText.enabled = false;
            _abilityCooldownImage.enabled = false;
            AbilityIndex = abilityIndex;
        }

        internal void SwitchCooldownState(bool isCoolingDown)
        {
            _abilityCooldownText.enabled = isCoolingDown;
            _abilityCooldownImage.enabled = isCoolingDown;
        }

        internal void UpdateUI(float cooldownLeft)
        {
            _abilityCooldownImage.fillAmount = cooldownLeft / _baseCooldown;
            _abilityCooldownText.text = ((int)cooldownLeft).ToString();
        }
        #endregion
    }
}