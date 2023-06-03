using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WatStudios.DeepestDungeon.UI.Gameplay
{
    public class BuffUI : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private Color _buffColor;
        [SerializeField] private Color _debuffColor;

        [Header("Components")]
        [SerializeField] private Image _buffImage;
        [SerializeField] private Outline _buffImageOutline;
        [SerializeField] private Image _cooldownBuffImage;
        [SerializeField] private TextMeshProUGUI _cooldownText;
        [SerializeField] private TextMeshProUGUI _buffStackText;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private bool _isStackable;
        private float _maxDuration;
        #endregion

        public void Init(Sprite buffSprite, bool isBuff, bool isStackable, float maxDuration)
        {
            _buffImage.sprite = buffSprite;
            if (isBuff)
                _buffImageOutline.effectColor = _buffColor;
            else
                _buffImageOutline.effectColor = _debuffColor;

            _buffStackText.enabled = _isStackable = isStackable;

            if (_isStackable)
                _buffStackText.text = "1";

            _maxDuration = maxDuration;
            _cooldownText.text = ((int)(_maxDuration)).ToString();
            _cooldownText.enabled = false;
            _cooldownBuffImage.fillAmount = _maxDuration/_maxDuration;
        }

        public void OnCooldownChange(float newDuration)
        {
            _cooldownText.text = ((int)(newDuration)).ToString();
            _cooldownBuffImage.fillAmount = newDuration/_maxDuration;
        }

        public void OnStackNumberChange(int newNumberOfStacks)
        {
            _buffStackText.text = newNumberOfStacks.ToString();
        }

        public void OnBuffRefresh(int numOfStacks)
        {
            if (_isStackable)
                OnStackNumberChange(numOfStacks);
            _cooldownText.text = _maxDuration.ToString();
        }

        public void Kill()
        {
            Destroy(gameObject);
        }
    }
}
