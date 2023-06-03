using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Networking;

namespace WatStudios.DeepestDungeon.Launcher
{
    public class Launcher : MonoBehaviour
    {
        #region Private Fields
        private Dictionary<string, RoomInfo> _cachedRoomList;
        private Dictionary<string, GameObject> _roomListEntries;
        private Dictionary<int, GameObject> _playerListEntries;

        private string _connectionIp;
        private bool _connectWithIp;
        #endregion

#pragma warning disable 649
        #region Exposed Private Fields
        [SerializeField, Tooltip("The Ui Panel to let the user enter name and connect")]
        private GameObject _mainMenuCanvas;
        [SerializeField, Tooltip("The input field that contains the player nickname")]
        private GameObject _logInCanvas;
        [SerializeField, Tooltip("The Ui Panel to let the user enter name and connect")]
        private GameObject _loginPanel;
        [SerializeField, Tooltip("The input field that contains the player nickname")]
        private InputField _playerNameInputField;
        [SerializeField, Tooltip("The UI Label to inform the user that the connection is in progress")]
        private GameObject _progressLabel;
        [SerializeField, Tooltip("The UI Panel to let the user chose a room to connect to")]
        private GameObject _choseRoomPanel;
        [SerializeField, Tooltip("The UI Panel that contains all available rooms")]
        private GameObject _availableRoomsPanel;
        [SerializeField, Tooltip("The input field that contains the name for a new room")]
        private InputField _roomNameInputField;
        [SerializeField, Tooltip("The text with the player name")]
        private Text _playerNameTextField;
        [SerializeField, Tooltip("The prefab for the roomListEntry")]
        private GameObject _roomListEntryPrefab;
        [SerializeField, Tooltip("The content gameobject of the scrollView")]
        private GameObject _roomListContent;

        #endregion
#pragma warning restore 649

        #region Public Fields
        public GameObject Loading;
        public GameObject Rooms;
        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            _cachedRoomList = new Dictionary<string, RoomInfo>();
            _roomListEntries = new Dictionary<string, GameObject>();
        }

        private void Start()
        {
            if (NetworkManager.Instance.IsConnected)
            {
                SetGroupActiveState(false, new[] { _progressLabel, _loginPanel, _mainMenuCanvas });
                SetGroupActiveState(true, new[] { _logInCanvas, _choseRoomPanel });
            }
            else
            {
                SetGroupActiveState(false, new[] { _logInCanvas, _progressLabel, _choseRoomPanel });
                SetGroupActiveState(true, new[] { _mainMenuCanvas, _loginPanel });
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnEnable()
        {
            MessageHub.Subscribe(MessageType.JoinedLobby, OnJoinedLobby, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.RoomListUpdated, OnRoomListUpdated, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.DisconnectedFromMaster, OnDisconnectedFromMaster, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.ReturnedToMenu, OnReturnedToMenu, ActionExecutionScope.UserInterface);
        }

        private void OnDisable()
        {
            MessageHub.Unsubscribe(MessageType.JoinedLobby, OnJoinedLobby);
            MessageHub.Unsubscribe(MessageType.RoomListUpdated, OnRoomListUpdated);
            MessageHub.Unsubscribe(MessageType.DisconnectedFromMaster, OnDisconnectedFromMaster);
            MessageHub.Unsubscribe(MessageType.ReturnedToMenu, OnReturnedToMenu);
        }
        #endregion

        #region MessageHub Callbacks
        private void OnReturnedToMenu(Message msg)
        {
            SetGroupActiveState(false, new[] { _loginPanel, _progressLabel });
            SetGroupActiveState(true, new[] { _choseRoomPanel });
        }

        private void OnJoinedLobby(Message obj)
        {
            SetGroupActiveState(false, new[] { _loginPanel, _progressLabel });
            SetGroupActiveState(true, new[] { _choseRoomPanel });
            _playerNameTextField.text = $"Logged in as {obj.Data[0] as string}";
        }

        private void OnRoomListUpdated(Message obj)
        {
            ClearRoomListView();
            UpdateCachedRoomList((List<RoomInfo>)obj.Data[0]);
            UpdateRoomListView();
        }

        private void OnDisconnectedFromMaster(Message obj)
        {
            SetGroupActiveState(false, new[] { _choseRoomPanel, _progressLabel });
            SetGroupActiveState(true, new[] { _loginPanel });
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Sets a group of gameobjects active according to <paramref name="active"/>
        /// </summary>
        /// <param name="active">True or false</param>
        /// <param name="gObjects"> Collection of gameobjects</param>
        private void SetGroupActiveState(bool active, GameObject[] gObjects)
        {
            foreach (var gObject in gObjects)
            {
                gObject.SetActive(active);
            }
        }

        /// <summary>
        /// Clears all entries from the ScrollView that contains the cached rooms
        /// </summary>
        private void ClearRoomListView()
        {
            foreach (GameObject entry in _roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            _roomListEntries.Clear();
        }

        /// <summary>
        /// Updates the local cached room list <paramref name="roomList"/>
        /// </summary>
        /// <param name="roomList">The new List of rooms</param>
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (var info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (_cachedRoomList.ContainsKey(info.Name))
                    {
                        _cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (_cachedRoomList.ContainsKey(info.Name))
                {
                    _cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    _cachedRoomList.Add(info.Name, info);
                }
            }
        }

        /// <summary>
        /// Updates the ScrollView with the cached room list
        /// </summary>
        private void UpdateRoomListView()
        {
            foreach (var info in _cachedRoomList.Values)
            {
                GameObject entry = Instantiate(_roomListEntryPrefab);
                entry.transform.SetParent(_roomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                _roomListEntries.Add(info.Name, entry);
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Connects to GameServer
        /// </summary>
        public void Connect()
        {
            SetGroupActiveState(false, new[] { _loginPanel, _choseRoomPanel });
            SetGroupActiveState(true, new[] { _progressLabel });
            MessageHub.SendMessage(MessageType.LauncherConnect);
            MessageHub.SendMessage(MessageType.LauncherNickname, _playerNameInputField.text);
        }

        /// <summary>
        /// Updates the connections setting with the corresponding IP address
        /// </summary>
        public void UpdateConnectionSettings(string ipAddress)
        {
            _connectionIp = ipAddress;
        }

        /// <summary>
        /// Updates the connections setting with the corresponding IP address
        /// </summary>
        public void UpdateConnectionSettings(bool connectWithIp)
        {
            _connectWithIp = connectWithIp;
        }

        /// <summary>
        /// Creates a room
        /// </summary>
        public void CreateRoom()
        {
            MessageHub.SendMessage(MessageType.LauncherCreateRoom, _roomNameInputField.text);
        }

        #endregion

        #region UI Callbacks

        /// <summary>
        /// Callback for UI Disconnect Button
        /// </summary>
        public void OnDisconnectButtonClicked()
        {
            MessageHub.SendMessage(MessageType.LauncherDisconnect);
        }

        public void OnQuitButtonPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        }
        #endregion
    }
}
