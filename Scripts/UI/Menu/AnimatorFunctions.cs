using UnityEngine;

namespace WatStudios.DeepestDungeon.UI.Menu
{
    public class AnimatorFunctions : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private MenuButtonController menuButtonController;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private bool _disableOnce;
        #endregion

        #region Private Methods
        private void PlaySound(AudioClip audioClip)
        {
            if (!_disableOnce)
            {
                menuButtonController.audioSource.PlayOneShot(audioClip);
            }
            else
            {
                _disableOnce = false;
            }
        }
        #endregion
    }
}