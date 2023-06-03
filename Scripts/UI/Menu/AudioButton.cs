using UnityEngine;

namespace WatStudios.DeepestDungeon.UI.Menu
{
    public class AudioButton : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private AudioSource _audioSource;
#pragma warning restore 649
        #endregion

        #region Private Methods
        private void PlaySound(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
        }
        #endregion
    }
}