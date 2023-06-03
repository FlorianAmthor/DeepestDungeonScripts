using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace WatStudios.DeepestDungeon.UI.Gameplay
{
    public class PlayerReadyUI : MonoBehaviour
    {
        #region Exposed Private Fields 
#pragma warning disable 649
        [SerializeField] private Image _playerReadyImage;
        [SerializeField] private Color _playerNotReadyColor;
        [SerializeField] private Color _playerReadyColor;
#pragma warning restore 649
        #endregion

        #region Private Methods
        internal void SetReady(bool isReady)
        {
            if (isReady)
                _playerReadyImage.color = _playerReadyColor;
            else
                _playerReadyImage.color = _playerNotReadyColor;
        }
        #endregion
    }
}
