using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WatStudios.DeepestDungeon.UI.Gameplay
{
    internal class PlayerGroupUI : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private Image _playerHealthImage;
        [SerializeField] private TextMeshProUGUI _playerHealthText;
        [SerializeField] private TextMeshProUGUI _playerNameText;
#pragma warning restore 649
        #endregion

        #region Internal Methods
        internal void SetPlayerHealthUI(float healthPercent)
        {
            _playerHealthImage.fillAmount = healthPercent;
            string healthPercentString = ((int)(healthPercent * 100.0f)).ToString();
            _playerHealthText.text = $"{healthPercentString}%";
        }
        internal void SetPlayerName(string playerName)
        {
            _playerNameText.text = playerName;
        }
        #endregion
    }
}