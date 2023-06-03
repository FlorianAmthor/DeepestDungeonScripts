using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Utility;

namespace WatStudios.DeepestDungeon.Networking
{
    public class NetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        #region Singleton
        public static NetworkManager Instance { get; private set; }

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
        [Header("Room Options")]
        [SerializeField, Tooltip("The maximum players per room. When a room is full it can't be joined by new players")] private byte _maxPlayersPerRoom;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private string _gameVersion;
        private Dictionary<Player, bool> _playerReadyDict;
        #endregion

        #region Public Field
        public bool IsConnected { get => PhotonNetwork.IsConnected; }
        public bool IsConnecting { get; private set; }
        public List<Player> PlayerList { get; private set; }
        public bool ReadyCheckActive { get; private set; }
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            _playerReadyDict = new Dictionary<Player, bool>();
            PlayerList = new List<Player>();
            PhotonNetwork.AutomaticallySyncScene = true;
            //SceneLoaderData.Reset();
        }

        public override void OnEnable()
        {
            if (this != Instance)
                return;
            PhotonNetwork.AddCallbackTarget(this);
            MessageHub.Subscribe(MessageType.LauncherConnect, OnLauncherConnect, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.LauncherConnectIp, OnLauncherConnectIp, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.LauncherDisconnect, OnLauncherDisconnect, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.LauncherCreateRoom, OnLauncherCreateRoom, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.LauncherJoinRoom, OnLauncherJoinRoom, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.LauncherNickname, OnLauncherNickname, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.MenuLeaveRoom, OnMenuLeaveRoom, ActionExecutionScope.Default);
        }

        public override void OnDisable()
        {
            if (this != Instance)
                return;
            PhotonNetwork.RemoveCallbackTarget(this);
            MessageHub.Unsubscribe(MessageType.LauncherConnect, OnLauncherConnect);
            MessageHub.Unsubscribe(MessageType.LauncherConnectIp, OnLauncherConnectIp);
            MessageHub.Unsubscribe(MessageType.LauncherDisconnect, OnLauncherDisconnect);
            MessageHub.Unsubscribe(MessageType.LauncherCreateRoom, OnLauncherCreateRoom);
            MessageHub.Unsubscribe(MessageType.LauncherJoinRoom, OnLauncherJoinRoom);
            MessageHub.Unsubscribe(MessageType.LauncherNickname, OnLauncherNickname);
            MessageHub.Unsubscribe(MessageType.MenuLeaveRoom, OnMenuLeaveRoom);
        }
        #endregion

        #region Messaging Callbacks
        private void OnLauncherConnectIp(Message msg)
        {
            object[] data = msg.Data;
            IsConnecting = true;
            if (IsConnected)
            {
                Debug.LogErrorFormat("Client already connected to server");
            }
            else
            {
                PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = Application.version;
                PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
                PhotonNetwork.PhotonServerSettings.AppSettings.Server = data[0] as string;
                PhotonNetwork.PhotonServerSettings.AppSettings.Port = (int)data[1];
                PhotonNetwork.PhotonServerSettings.AppSettings.Protocol = ConnectionProtocol.Udp;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        private void OnLauncherConnect(Message msg)
        {
            IsConnecting = true;
            if (IsConnected)
            {
                Debug.LogErrorFormat("Client already connected to server");
            }
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = Application.version;
            PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = "";
            PhotonNetwork.ConnectUsingSettings();
        }

        private void OnLauncherDisconnect(Message msg)
        {
            PhotonNetwork.Disconnect();
        }

        private void OnLauncherCreateRoom(Message msg)
        {
            object[] data = msg.Data;
            PhotonNetwork.CreateRoom(data[0] as string, new RoomOptions { MaxPlayers = _maxPlayersPerRoom, CleanupCacheOnLeave = true, PlayerTtl = 0 });
        }

        private void OnLauncherJoinRoom(Message msg)
        {
            PhotonNetwork.JoinRoom(msg.Data[0] as string);
        }

        private void OnLauncherNickname(Message msg)
        {
            PhotonNetwork.NickName = msg.Data[0] as string;
        }
        private void OnMenuLeaveRoom(Message msg)
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region PUN Callbacks
        public override void OnConnectedToMaster()
        {
            Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");
            if (IsConnecting)
            {
                PhotonNetwork.AuthValues = new AuthenticationValues(PhotonNetwork.NickName);
            }
            PhotonNetwork.JoinLobby();
        }

        //TODO: Thread needs to be killed when game crashes
        public void KeepAlive()
        {
            PhotonNetwork.IsMessageQueueRunning = false;
            while ((Thread.CurrentThread.ThreadState & ThreadState.AbortRequested) != ThreadState.AbortRequested)
            {
                PhotonNetwork.SendAllOutgoingCommands();
                Thread.Sleep(150);
            }
            PhotonNetwork.IsMessageQueueRunning = true;
        }

        public override void OnJoinedLobby()
        {
            IsConnecting = false;
            PhotonNetwork.GetCustomRoomList(TypedLobby.Default, null);
            MessageHub.SendMessage(MessageType.JoinedLobby, PhotonNetwork.NickName);
            Debug.Log("Launcher: Joined Lobby.");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("Launcher: Rooms Updated");
            MessageHub.SendMessage(MessageType.RoomListUpdated, roomList);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
            MessageHub.SendMessage(MessageType.DisconnectedFromMaster);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Launcher: Connected to a room.");
            Debug.Log("Load Game Scene");
            PlayerList.Clear();
            _playerReadyDict.Clear();
            PlayerList = PhotonNetwork.PlayerList.ToList();
            foreach (var player in PlayerList)
            {
                _playerReadyDict.Add(player, false);
            }
            PhotonNetwork.LoadLevel(1); //Buildindex of loading screen scene
            PhotonNetwork.LeaveLobby();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Launcher:OnJoinRoomFailed(). No room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            PlayerList.Clear();
            _playerReadyDict.Clear();
            SceneManager.LoadScene(0);
            PhotonNetwork.JoinLobby();
            MessageHub.SendMessage(MessageType.ReturnedToMenu);
        }

        //This doesn't get called for the local player
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (!PlayerList.Contains(newPlayer))
            {
                PlayerList.Add(newPlayer);
                _playerReadyDict.Add(newPlayer, false);
                MessageHub.SendMessage(MessageType.PlayerJoinedRoom, newPlayer);
            }
            Debug.LogFormat($"{newPlayer.NickName} entered the room");

            if (ReadyCheckActive)
                HandleReadyCheckEnd(MessageType.ReadyCheckFail);
        }

        public override void OnPlayerLeftRoom(Player leavingPlayer)
        {
            PlayerList.Remove(leavingPlayer);
            _playerReadyDict.Remove(leavingPlayer);
            MessageHub.SendMessage(MessageType.PlayerLeftRoom, leavingPlayer);
            Debug.LogFormat($"{leavingPlayer.NickName} left the room");
            if (ReadyCheckActive)
                HandleReadyCheckEnd(MessageType.ReadyCheckFail);
        }

        public bool RaiseNetworkEvent(NetworkGameEventCode gameEvent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions, params object[] content)
        {
            return PhotonNetwork.RaiseEvent((byte)gameEvent, content, raiseEventOptions, sendOptions);
        }

        public void OnEvent(EventData photonEvent)
        {
            switch ((NetworkGameEventCode)photonEvent.Code)
            {
                case NetworkGameEventCode.ReadyCheckResponse:
                    object[] data = (object[])photonEvent.CustomData;
                    bool isReady = (bool)data[0];
                    Player player = PlayerList.Find(p => p.ActorNumber == photonEvent.Sender);
                    if (!isReady)
                    {
                        HandleReadyCheckEnd(MessageType.ReadyCheckFail);
                    }
                    else
                    {
                        _playerReadyDict[player] = isReady;
                        if (_playerReadyDict.All(entry => entry.Value == true))
                            HandleReadyCheckEnd(MessageType.AllPlayersReady);
                        else
                            MessageHub.SendMessage(MessageType.ReadyCheckResponse, player, isReady);
                    }
                    break;
                case NetworkGameEventCode.MonsterDeath:
                    MessageHub.SendMessage(MessageType.MonsterDeath);
                    break;
                case NetworkGameEventCode.ReadyCheckInit:
                    ReadyCheckActive = true;
                    MessageHub.SendMessage(MessageType.ReadyCheckInit);
                    break;
                case NetworkGameEventCode.GamePlaySceneBuilt:
                    MessageHub.SendMessage(MessageType.GamePlaySceneBuilt);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Private Methods
        private void HandleReadyCheckEnd(MessageType messageType)
        {
            foreach (var p in PlayerList)
            {
                _playerReadyDict[p] = false;
            }
            ReadyCheckActive = false;
            MessageHub.SendMessage(messageType);
        }
        #endregion
    }
}