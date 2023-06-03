using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance { get; private set; }

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
        [SerializeField]
        private GameObject[] _dontDestroyOnLoadSingletons;
        [SerializeField, Tooltip("The prefab to use for representing the player")]
        private GameObject _dmgPlayerPrefab, _tankPlayerPrefab, _healPlayerPrefab;
        [SerializeField, Tooltip("The build index of the scene to be loaded when the players pass the ready check")]
        private int _gameplaySceneBuildIndex;
        [SerializeField]
        private GameObject _lobbySpawnPosition;
        [SerializeField]
        private float _respawnTime;
        [SerializeField] private int _monstersToKill;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private List<GameObject> _objectsToCleanUpOnSceneChange;
        private Thread _thread;
        private bool started;
        private int _currentMonsterKillCount;
        #endregion

        #region Properties
        public Dictionary<Player, NetworkPlayer> PlayerDictionary { get; private set; }
        public float RespawnTime { get => _respawnTime; }
        public Vector3 GameplaySpawnPosition { get; private set; }
        #endregion

        #region MonoBehaviour Callbacks

        private void Init()
        {
            PlayerDictionary = new Dictionary<Player, NetworkPlayer>();
            if (_dmgPlayerPrefab == null || _healPlayerPrefab == null || _tankPlayerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (NetworkPlayer.LocalPlayerInstance == null)
                {
                    GameObject playerPrefab = null;
                    switch (ChosenPlayerClass)
                    {
                        case PlayerClass.Dmg:
                            playerPrefab = _dmgPlayerPrefab;
                            break;
                        case PlayerClass.Tank:
                            playerPrefab = _tankPlayerPrefab;
                            break;
                        case PlayerClass.Healer:
                            playerPrefab = _healPlayerPrefab;
                            break;
                        default:
                            break;
                    }
                    _objectsToCleanUpOnSceneChange.Add(PhotonNetwork.Instantiate(playerPrefab.name, _lobbySpawnPosition.transform.position, Quaternion.identity, 0));
                }
            }

            _objectsToCleanUpOnSceneChange.Add(gameObject);

        }

        public void StartGame()
        {
            _objectsToCleanUpOnSceneChange = new List<GameObject>();
            foreach (GameObject singleton in _dontDestroyOnLoadSingletons)
            {
                _objectsToCleanUpOnSceneChange.Add(Instantiate(singleton));
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
            //RegisterCustomTypes();
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 1 && !started)
            {
                StartGame(); // TODO Find A Better way than  the Update to Initiate after it loaded to the Game Scene.
                Init();
                OnDisable(); // TODO Finding a Better way to First registrate the Abilities. But like this it works for now. But When using the Abilites they are still not registrated.
                OnEnable();
                started = true;
            }
        }

        private void OnEnable()
        {
            if (this != Instance)
                return;
            MessageHub.Subscribe(MessageType.AllPlayersReady, OnAllPlayersReady, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.ReturnedToMenu, OnReturnToMenu, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.PlayerLeftRoom, OnPlayerLeftRoom, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.PlayerInstantiate, OnPlayerInstantiate, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.SpawnPoint, OnSpawnPointReceived, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.GamePlaySceneBuilt, OnGamePlaySceneBuilt, ActionExecutionScope.Gameplay);
            MessageHub.Subscribe(MessageType.MonsterDeath, OnMonsterDeath, ActionExecutionScope.Gameplay);
        }

        private void OnDisable()
        {
            if (this != Instance)
                return;
            MessageHub.Unsubscribe(MessageType.AllPlayersReady, OnAllPlayersReady);
            MessageHub.Unsubscribe(MessageType.ReturnedToMenu, OnReturnToMenu);
            MessageHub.Unsubscribe(MessageType.PlayerLeftRoom, OnPlayerLeftRoom);
            MessageHub.Unsubscribe(MessageType.PlayerInstantiate, OnPlayerInstantiate);
            MessageHub.Unsubscribe(MessageType.SpawnPoint, OnSpawnPointReceived);
            MessageHub.Unsubscribe(MessageType.GamePlaySceneBuilt, OnGamePlaySceneBuilt);
            MessageHub.Unsubscribe(MessageType.MonsterDeath, OnMonsterDeath);

        }
        #endregion

        public static PlayerClass ChosenPlayerClass;

        #region Messaging Callbacks
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex == _gameplaySceneBuildIndex - 1)
            {
                MessageHub.SendMessage(MessageType.LobbySceneLoaded);
                if (NetworkPlayer.LocalPlayerInstance)
                    NetworkPlayer.LocalPlayerInstance.transform.position = _lobbySpawnPosition.transform.position;
            }
            if (arg0.buildIndex == _gameplaySceneBuildIndex)
            {
                MessageHub.SendMessage(MessageType.GameplaySceneLoaded);
                MessageHub.SendMessage(MessageType.AllowPlayerInput, true);
                PhotonNetwork.AutomaticallySyncScene = false;
                //TODO: Set PlayerPosition according to world gen spawn
                if (NetworkPlayer.LocalPlayerInstance)
                    NetworkPlayer.LocalPlayerInstance.transform.position = Vector3.zero;
            }
            //NetworkPlayer.LocalPlayerInstance.PhotonPlayer.SetCustomProperties(new Hashtable { { "CurrentSceneIndex", arg0.buildIndex } });
        }

        private void OnReturnToMenu(Message obj)
        {
            CleanUpSingletons();
        }

        private void OnPlayerInstantiate(Message obj)
        {
            NetworkPlayer nPlayer = obj.Data[0] as NetworkPlayer;
            Player photonPlayer = obj.Data[1] as Player;
            PlayerDictionary.Add(photonPlayer, nPlayer);
        }

        private void OnPlayerLeftRoom(Message obj)
        {
            PlayerDictionary.Remove(obj.Data[0] as Player);
        }

        private void OnAllPlayersReady(Message obj)
        {
            MessageHub.SendMessage(MessageType.AllowPlayerInput, false);

            //_thread = new Thread(new ThreadStart(NetworkManager.Instance.KeepAlive));
            //_thread.IsBackground = true;
            //_thread.Start();
            //SceneLoaderData.SetData(_gameplaySceneBuildIndex);
            //TODO: Show Loading Screen Overlay

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(_gameplaySceneBuildIndex);
        }

        private void OnSpawnPointReceived(Message obj)
        {
            GameplaySpawnPosition = (Vector3)obj.Data[0];

            if (NetworkPlayer.LocalPlayerInstance)
            {
                NetworkPlayer.LocalPlayerInstance.transform.position = GameplaySpawnPosition;
                NetworkPlayer.LocalPlayerInstance.wasTeleported = true;
            }
        }

        private void OnGamePlaySceneBuilt(Message obj)
        {
            //_thread.Abort();
            MessageHub.SendMessage(MessageType.KillObjectiveData, _currentMonsterKillCount, _monstersToKill);
        }

        private void OnMonsterDeath(Message obj)
        {
            _currentMonsterKillCount = Mathf.Clamp(_currentMonsterKillCount++, 0, _monstersToKill);
            MessageHub.SendMessage(MessageType.KillObjectiveData, _currentMonsterKillCount, _monstersToKill);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Deletes the Singletons needed for the actual game
        /// </summary>
        private void CleanUpSingletons()
        {
            foreach (GameObject item in _objectsToCleanUpOnSceneChange)
            {
                Destroy(item);
            }
        }

        public void RegisterCustomTypes()
        {
            //Not needed anymore but we'll keep it in case we need to sync custom types
            //PhotonPeer.RegisterType(typeof(AttackState), (byte)PhotonCustomTypes.AttackState, (object obj) => { return new byte[] { }; }, (byte[] data) => { return new AttackState(); });
            //PhotonPeer.RegisterType(typeof(ChaseState), (byte)PhotonCustomTypes.ChaseState, (object obj) => { return new byte[] { }; }, (byte[] data) => { return new ChaseState(); });
            //PhotonPeer.RegisterType(typeof(IdleState), (byte)PhotonCustomTypes.IdleState, (object obj) => { var s = (IdleState)obj; return BitConverter.GetBytes(s.MaxIdleDistance); }, (byte[] data) => { var s = new IdleState(); s.MaxIdleDistance = BitConverter.ToSingle(data, 0); return s; });
        }
        #endregion
    }
}
