using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WatStudios.DeepestDungeon.Messaging;

namespace WatStudios.DeepestDungeon.Options
{
    public class MouseSettings : MonoBehaviour
    {
        [SerializeField] private Slider _mouseSlider;

        private void Start()
        {
            _mouseSlider.value = PlayerPrefs.GetFloat("mouseSensitivity", 0.5f);
        }
        private void OnEnable()
        {
            _mouseSlider.value = PlayerPrefs.GetFloat("mouseSensitivity", 0.5f);
        }

        public void OnSensitivtyChanged()
        {
            PlayerPrefs.SetFloat("mouseSensitivity", _mouseSlider.value);
            MessageHub.SendMessage(MessageType.SensitivityChanged, _mouseSlider.value);
        }

        private void OnDisable()
        {
            PlayerPrefs.Save();
        }
    }
}