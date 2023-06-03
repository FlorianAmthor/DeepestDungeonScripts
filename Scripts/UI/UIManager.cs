using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;

namespace WatStudios.DeepestDungeon.UI
{
    public class UIManager : MonoBehaviour
    {
        #region Singleton
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private Canvas _menuCanvas;
#pragma warning restore 649
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            _menuCanvas.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                SwitchMenuState();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Switches the active state of the console window
        /// </summary>
        public void SwitchMenuState()
        {
            _menuCanvas.gameObject.SetActive(!_menuCanvas.gameObject.activeInHierarchy);
            MessageHub.SendMessage(MessageType.AllowPlayerInput, !_menuCanvas.gameObject.activeInHierarchy);
        }
        #endregion

        #region Public Methods
        public void OnLeaveRoomButtonClicked()
        {
            MessageHub.SendMessage(MessageType.MenuLeaveRoom);
        }
        #endregion

    }
}
