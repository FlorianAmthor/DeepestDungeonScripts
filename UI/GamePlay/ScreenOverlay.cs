using UnityEngine;
using UnityEngine.UI;
using WatStudios.DeepestDungeon.Messaging;

namespace WatStudios.DeepestDungeon.UI.Gameplay
{
    public class ScreenOverlay : MonoBehaviour
    {
        public bool start; // TODO Just For Test Purpose. Can be deleted After Tests are Over.
        [Range(0, 1)] public float health; // TODO Just For Test Purpose. Can be deleted After Tests are Over.

        [Header("For Animation")]
        [SerializeField] private Sprite[] _overlayAnimSprites;
        [SerializeField] private Image _overlayAnimPanel;

        [Header("For Fading")]
        [SerializeField] private float _fadeTime;
        [SerializeField] private Sprite _overlayFadeSprites;
        [SerializeField] private Image _overlayFadePanel;

        private int _maxAmount;
        private float _timer;
        private bool _startFullFade;

        // Start is called before the first frame update
        void Start()
        {
            _maxAmount = _overlayAnimSprites.Length;

            if (_overlayAnimSprites[0] != null && _overlayAnimPanel != null)
                _overlayAnimPanel.sprite = _overlayAnimSprites[0];

            if (_overlayFadeSprites != null && _overlayFadePanel != null)
            {
                _overlayFadePanel.sprite = _overlayFadeSprites;

                Color col = _overlayFadePanel.color;
                col.a = 0;
                _overlayFadePanel.color = col;
            }
        }

        private void Update() // TODO Just For Test Purpose. Can be deleted After Tests are Over.
        {
            if (start)
                FadeOverlay();
            else
                UpdateOverlayFade();

            UpdateOverlayAnim(health);
        }

        /// <summary>
        /// Updates the Sprite accourding to the percentage. Percentage should be a value between 0.0 and 1.0
        /// </summary>
        /// <param name="percentage"></param>
        public void UpdateOverlayAnim(float percentage)
        {
            int tmpAmount = Mathf.RoundToInt((percentage) * _maxAmount);

            if (tmpAmount >= _maxAmount)
                tmpAmount = _maxAmount - 1;

            _overlayAnimPanel.sprite = _overlayAnimSprites[tmpAmount];
        }

        public void FadeOverlay()
        {
            Color col = _overlayFadePanel.color;
            col.a = 1;
            _overlayFadePanel.color = col;
            _startFullFade = true;
            start = false; // TODO Just For Test Purpose. Can be deleted After Tests are Over.
        }

        private void UpdateOverlayFade()
        {
            if (_startFullFade)
            {
                _timer = 0;
                _startFullFade = false;
            }
            else if (!_startFullFade && _timer <= _fadeTime)
            {
                _timer += Time.deltaTime;

                Color col = _overlayFadePanel.color;
                col.a = 1 - (_timer / _fadeTime);

                if (col.a <= 0)
                    col.a = 0;

                _overlayFadePanel.color = col;
            }
        }
    }
}
