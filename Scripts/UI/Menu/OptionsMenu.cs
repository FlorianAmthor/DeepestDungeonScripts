using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WatStudios.DeepestDungeon.Options
{

    public class OptionsMenu : MonoBehaviour
    {
        //AudioSlider in "AudioSettings" script

        public TMP_Dropdown resolutionDropdown;
        public Slider _sensitivitySlider;
        Resolution[] resolutions;
        

        void Start()
        {
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            PlayerPrefs.SetFloat("mouseSensitivity", 0.5f);
            _sensitivitySlider.value = PlayerPrefs.GetFloat("mouseSensitivity", 0.5f);
            PlayerPrefs.Save();
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        public void SetQuality(int qualityindex)
        {
            QualitySettings.SetQualityLevel(qualityindex);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetMouseSensitivity(float newSensitivity)
        {
            PlayerPrefs.SetFloat("mouseSensitivity", newSensitivity);
            PlayerPrefs.Save();
            //TODO: Send changed value over MessageHub
        }
    }
}
