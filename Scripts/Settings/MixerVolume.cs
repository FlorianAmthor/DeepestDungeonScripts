using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace WatStudios.DeepestDungeon.Options
{
    [System.Serializable]
    public class MixerVolume
    {
        #region Exposed Private 
#pragma warning disable 649
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private Slider slider;
#pragma warning restore 649
        #endregion

        #region Properties
        public Slider Slider { get { return slider; } }
        public AudioMixerGroup MixerGroup { get { return mixerGroup; } }
        #endregion
    }
}