using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.ThreatSystem
{
    public class ThreatManager : MonoBehaviourPun, IPunObservable
    {
        #region Private Exposed Fields
#pragma warning disable 649
        [SerializeField] private ThreatManagerConfig _threatManagerConfig;
        [SerializeField] private EnemyEntity _owner;
#pragma warning restore 649
        #endregion

        #region Properties
        internal Dictionary<NetworkPlayer, PlayerThreatData> PlayerThreatDictionary { get; private set; }
        internal ThreatManagerConfig ThreatManagerConfig => _threatManagerConfig;
        #endregion

        #region MonoBehaviour Callbacks
        internal void Tick()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            if (PlayerThreatDictionary.Count > 0)
            {
                photonView.RPC("UpdateThreatList", RpcTarget.AllBuffered);
                photonView.RPC("TargetHighestThreat", RpcTarget.AllBuffered);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                var networkPlayer = other.GetComponent<NetworkPlayer>();
                if (!PlayerThreatDictionary.ContainsKey(networkPlayer))
                {
                    PlayerThreatData p = new PlayerThreatData(_threatManagerConfig.BaseThreat);
                    PlayerThreatDictionary.Add(networkPlayer, p);
                }
            }
        }

        private void OnEnable()
        {
            PlayerThreatDictionary = new Dictionary<NetworkPlayer, PlayerThreatData>();
            foreach (var player in GameManager.Instance.PlayerDictionary.Values)
            {
                PlayerThreatDictionary.Add(player, new PlayerThreatData(0));
            }

            MessageHub.Subscribe(MessageType.PlayerJoinedRoom, OnPlayerJoinRoom, ActionExecutionScope.Gameplay);
            MessageHub.Subscribe(MessageType.PlayerLeftRoom, OnPlayerLeftRoom, ActionExecutionScope.Gameplay);
        }

        private void OnDisable()
        {
            MessageHub.Unsubscribe(MessageType.PlayerJoinedRoom, OnPlayerJoinRoom);
            MessageHub.Unsubscribe(MessageType.PlayerLeftRoom, OnPlayerLeftRoom);
        }
        #endregion

        #region Messaging Callbacks
        private void OnPlayerJoinRoom(Message obj)
        {
            Player player = obj.Data[0] as Player;
            if (GameManager.Instance.PlayerDictionary.TryGetValue(player, out NetworkPlayer nPlayer))
            {
                if (!PlayerThreatDictionary.ContainsKey(nPlayer))
                    PlayerThreatDictionary.Add(nPlayer, new PlayerThreatData(0));
            }
            else
                Debug.LogError($"No such player: {player}");
        }

        private void OnPlayerLeftRoom(Message obj)
        {
            Player player = obj.Data[0] as Player;
            if (GameManager.Instance.PlayerDictionary.TryGetValue(player, out NetworkPlayer nPlayer))
                PlayerThreatDictionary.Remove(nPlayer);
            else
                Debug.LogError($"No such player: {player}");
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set the target to the player with the highest Threat
        /// </summary>
        [PunRPC]
        internal void TargetHighestThreat()
        {
            if (PlayerThreatDictionary.Count == 0)
            {
                PlayerThreatDictionary.Clear();
                _owner.TargetPlayer = null;
                return;
            }

            KeyValuePair<NetworkPlayer, PlayerThreatData> maxPair = PlayerThreatDictionary.First();

            foreach (var item in PlayerThreatDictionary)
            {
                if (item.Value.CompareTo(maxPair.Value) > 0)
                {
                    maxPair = item;
                }
            }

            switch (maxPair.Value.Priority)
            {
                case Priority.Untargetable:
                    var newData = maxPair.Value;
                    newData.Threat = 0;
                    PlayerThreatDictionary[maxPair.Key] = newData;
                    _owner.TargetPlayer = null;
                    break;
                case Priority.Normal:
                    if (maxPair.Value.Threat > 0)
                        _owner.TargetPlayer = maxPair.Key;
                    break;
                case Priority.High:
                    _owner.TargetPlayer = maxPair.Key;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Updates the ThreatList with new Threat Values
        /// </summary>
        [PunRPC]
        private void UpdateThreatList()
        {
            var keys = new List<NetworkPlayer>(PlayerThreatDictionary.Keys);
            foreach (var key in keys)
            {
                PlayerThreatData p = PlayerThreatDictionary[key];
                var threatMult = key.CurrentStats.ThreatMultiplier.Value;
                p.Threat = GeneratePlayerThreat(p, threatMult);
                p.DmgInFrame = 0;
                p.HealInFrame = 0;
                PlayerThreatDictionary[key] = p;
            }
        }

        /// <summary>
        /// Calculates the new Threat for the Player based on <paramref name="playerThreatData"/> and <paramref name="player"/>
        /// </summary>
        /// <param name="playerThreatData">Struct with Threat and Damage Values in this frame</param>
        /// <param name="player">The player Object</param>
        /// <returns></returns>
        private int GeneratePlayerThreat(PlayerThreatData playerThreatData, float threatMult)
        {
            int playerThreat = playerThreatData.Threat;
            playerThreat = (int)(ReduceThreatPerSeconds(playerThreat) + (playerThreatData.DmgInFrame + playerThreatData.HealInFrame / 2) * threatMult);
            return playerThreat;
        }

        /// <summary>
        /// Calculates the new threat with _percentualThreatLossPerSecond attribute
        /// </summary>
        /// <param name="threat">Current threat</param>
        /// <returns>Updated Threat Value</returns>
        private int ReduceThreatPerSeconds(int threat)
        {
            var newThreat = (int)(threat - (threat * _threatManagerConfig.PercentualThreatLossPerSecond * Time.deltaTime));
            if (newThreat < 0)
                newThreat = 0;
            return newThreat;
        }
        #endregion

        #region Internal Methods
        internal bool AddPlayer(NetworkPlayer nPlayer, PlayerThreatData pData = new PlayerThreatData())
        {
            pData.Reset();
            if (!PlayerThreatDictionary.ContainsKey(nPlayer))
            {
                PlayerThreatDictionary.Add(nPlayer, pData);
                return true;
            }
            return false;
        }

        internal bool UpdatePlayer(NetworkPlayer nPlayer, Priority priority)
        {
            if (PlayerThreatDictionary.TryGetValue(nPlayer, out PlayerThreatData pData))
            {
                pData.Priority = priority;
                PlayerThreatDictionary[nPlayer] = pData;
                return true;
            }
            else
                return false;
        }

        internal bool UpdatePlayer(NetworkPlayer nPlayer, int dmgInFrame = 0, int healInFrame = 0)
        {
            if (PlayerThreatDictionary.TryGetValue(nPlayer, out PlayerThreatData pData))
            {
                pData.DmgInFrame = dmgInFrame;
                pData.HealInFrame = healInFrame;
                PlayerThreatDictionary[nPlayer] = pData;
                return true;
            }
            else
                return false;
        }

        internal bool UpdatePlayer(NetworkPlayer nPlayer, int threat)
        {
            if (PlayerThreatDictionary.TryGetValue(nPlayer, out PlayerThreatData pData))
            {
                pData.Threat = threat;
                PlayerThreatDictionary[nPlayer] = pData;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the player with the highest threat and priority
        /// </summary>
        /// <returns>Returns a Pair<null,0> if the enemy does'nt have a targetplayer </returns>
        internal KeyValuePair<NetworkPlayer, PlayerThreatData> GetHighestThreatPlayer()
        {
            if (_owner.TargetPlayer == null)
                return new KeyValuePair<NetworkPlayer, PlayerThreatData>(null, new PlayerThreatData(Priority.Untargetable, 0));
            else
                return new KeyValuePair<NetworkPlayer, PlayerThreatData>(_owner.TargetPlayer, PlayerThreatDictionary[_owner.TargetPlayer]);
        }
        #endregion

        #region IPunObservable Implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (stream.IsWriting)
                {
                    //stream.SendNext(PlayerThreatDictionary);
                }
            }
            else
            {
                //PlayerThreatDictionary = stream.ReceiveNext() as Dictionary<NetworkPlayer, PlayerThreatData>;
            }
        }
        #endregion
    }
}
