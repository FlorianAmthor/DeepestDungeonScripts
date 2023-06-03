using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
namespace WatStudios.DeepestDungeon.Launcher
{
    public class SimpleLoadingBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.LevelLoadingProgress != 0)
            {
                float progress = Mathf.Clamp01(PhotonNetwork.LevelLoadingProgress / 0.9f);
                slider.value = progress;
            }
            else
                slider.value = 0;
        }
    }
}