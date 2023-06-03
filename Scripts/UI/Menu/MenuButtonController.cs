using UnityEngine;

namespace WatStudios.DeepestDungeon.UI.Menu
{
    public class MenuButtonController : MonoBehaviour
    {
        public AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayButtonPressed()
        {
            Debug.Log("Play!");
        }

        public void OptionsButtonPressed()
        {
            Debug.Log("Options!");
        }

        public void QuitButtonPressed()
        {
            Debug.Log("Quit!");
            Application.Quit();
        }
    }
}