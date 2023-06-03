using UnityEngine;
using UnityEngine.UI;

namespace WatStudios.DeepestDungeon.UI.Gameplay
{
    internal class SimpleDynamicCrosshair : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField]
        private RectTransform _recticle; // The RecTransform of recticle UI element.
        [SerializeField] private float _maxSize;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private float _currentSize;
        #endregion

        #region Public Fields
        
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            if (_recticle == null)
                _recticle = GetComponent<RectTransform>();
        }
        #endregion

        #region Internal Methods
        internal void ChangeSize(float spread)
        {
            _currentSize = _maxSize * (spread * 4);
            _recticle.sizeDelta = new Vector2(_currentSize, _currentSize);
        }

        internal void ResetToZero()
        {
            _recticle.sizeDelta = new Vector2(0f, 0f);
        }

        internal void SetVisibility(bool visible)
        {
            var images = GetComponentsInChildren<Image>();
            foreach (var image in images)
            {
                image.enabled = visible;
            }
        }
        #endregion
    }
}