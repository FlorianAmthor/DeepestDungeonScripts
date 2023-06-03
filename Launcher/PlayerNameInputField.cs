using UnityEngine;
using UnityEngine.UI;
using WatStudios.DeepestDungeon.Messaging;

namespace WatStudios.DeepestDungeon.Launcher
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {

        #region private Constants

        private const string PlayerName = "Playername";

        #endregion


        #region MonoBehaviour CallBacks

        private void Start()
        {
            var inputField = GetComponent<InputField>();

            if (inputField)
            {
                if (PlayerPrefs.HasKey(PlayerName))
                {
                    string playerName = PlayerPrefs.GetString(PlayerName);
                    inputField.text = playerName;
                }
            }
        }

        #endregion

        #region Public Methods

        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player name is null or empty");
                return;
            }
            PlayerPrefs.SetString(PlayerName, value);
        }

        #endregion
    }
}