using UnityEngine;
using WatStudios.DeepestDungeon.Audio;

namespace WatStudios.DeepestDungeon.Options
{
    public class AudioSettings : MonoBehaviour
    {
        #region Exposed Private Methods
#pragma warning disable 649
        [SerializeField] private MixerVolume[] volumeGroups;
        [Tooltip("Default value is 0.")]
        [SerializeField] private float maxVolume = 0;
        [Tooltip("Default value is -80.")]
        [SerializeField] private float minVolume = -80;
        [Tooltip("Should be between the max and the min value.")]
        [SerializeField] private float defaultVolume = -20;
#pragma warning restore 649
        #endregion

        #region MonoBehaviour Callbacks
        private void OnEnable()
        {
            foreach (MixerVolume vol in volumeGroups)
            {
                vol.Slider.value = GetVolume(vol);

                vol.Slider.minValue = minVolume;
                vol.Slider.maxValue = maxVolume;
            }
        }

        private void Start()
        {
            foreach (MixerVolume vol in volumeGroups)
            {
                vol.Slider.value = GetVolume(vol);

                vol.Slider.minValue = minVolume;
                vol.Slider.maxValue = maxVolume;
            }
        }
        #endregion

        #region Public Methods
        public void OnAudioVolumeChanged()
        {
            foreach (MixerVolume vol in volumeGroups)
                AudioManager.Instance.Mixer.SetFloat(vol.MixerGroup.name, vol.Slider.value);
        }

        public void SetDefaults()
        {
            foreach (MixerVolume vol in volumeGroups)
            {
                vol.Slider.value = defaultVolume;
                AudioManager.Instance.Mixer.SetFloat(vol.MixerGroup.name, defaultVolume);
            }
        }

        public float GetVolume(MixerVolume vol)
        {
            float value;
            bool result = AudioManager.Instance.Mixer.GetFloat(vol.MixerGroup.name, out value);
            if (result)
                return value;
            else
                return defaultVolume;
        }
        #endregion
    }
}