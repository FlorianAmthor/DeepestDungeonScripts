using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects;
using WatStudios.DeepestDungeon.Core.Attributes;
using WatStudios.DeepestDungeon.Core.EnemyLogic.ThreatSystem;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Networking;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic
{
    public class EnemyEntity : MonoBehaviourPun, IPunObservable, IStatusEntity
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private Animator _anim;
        [SerializeField] private EnemyBaseStats _baseStats;
        [SerializeField] private List<EnemyTrait> _traits;
        [SerializeField, Tooltip("Factor how far the enemy should move into its attacking range before attacking")]
        private float _attackRangePerecentage;
        [SerializeField] private float _timeBeforeDestroy;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private ThreatManager _threatManager;
        private Vector3 _deathBroadCastPosition;
        private Dictionary<Type, EnemyTrait> _traitDictionary;
        #endregion

        #region Properties
        public EnemyCurrentStats CurrentStats { get; protected set; }
        internal EnemyBaseStats BaseStats => _baseStats;
        internal EnemyBehaviour EnemyBehaviour { get; private set; }
        public ThreatManager ThreatManager => _threatManager;
        public StatusEffectHandler StatusEffectHandler { get; private set; }
        public PhotonView PhotonView => photonView;
        public NetworkPlayer TargetPlayer { get; internal set; }
        public Animator Animator { get => _anim; }
        public NavMeshAgent NavMeshAgent { get; private set; }
        public float AttackRangePercentage => _attackRangePerecentage;
        #endregion

        #region MonoBehaviour Callbacks
        private void OnEnable()
        {
            _traitDictionary = new Dictionary<Type, EnemyTrait>();
            CurrentStats = new EnemyCurrentStats(_baseStats);
            _threatManager = GetComponent<ThreatManager>();
            StatusEffectHandler = GetComponent<StatusEffectHandler>();
            EnemyBehaviour = GetComponent<EnemyBehaviour>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            //TODO: How to handle enemy traits Enemies DEV<3>

            foreach (var trait in _traits)
            {
                _traitDictionary.Add(trait.GetType(), trait);
            }
            GetComponent<SphereCollider>().radius = CurrentStats.AwarenessRadius.Value;

            NavMeshAgent.speed = CurrentStats.MoveSpeed.Value;
            NavMeshAgent.stoppingDistance = CurrentStats.AttackRange.Value * _attackRangePerecentage;
            NavMeshAgent.updateRotation = true;

            _anim.SetBool("Activated", true);
        }
        #endregion

        #region Internal Methods
        internal void Tick()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            ThreatManager.Tick();
            StatusEffectHandler.Tick();
            EnemyBehaviour.Tick();

            //TODO: Create a StatusEffect for Rooted
            //if (CurrentStats.IsRooted.Value)
            //{
            //    NavMeshAgent.isStopped = true;
            //    CurrentStats.IsRooted.Tick();
            //}
            //else if (!CurrentStats.IsRooted.Value && NavMeshAgent.isStopped)
            //{
            //    NavMeshAgent.isStopped = false;
            //}
        }


        private void OnTriggerEnter(Collider other)
        {
            if (EnemyManager.Instance.IsEnemyActive(this))
            {
                if (other.tag == "Player" && _threatManager.PlayerThreatDictionary.All(pair => pair.Value.Threat == 0))
                {
                    _threatManager.UpdatePlayer(other.GetComponent<NetworkPlayer>(), _threatManager.ThreatManagerConfig.BaseThreat);
                }
            }
        }
        #endregion

        #region Private Methods      

        /// <summary>
        /// Sends OnDeath events and disables enemy behaviours
        /// </summary>
        private void Die()
        {
            RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            NetworkManager.Instance.RaiseNetworkEvent(NetworkGameEventCode.MonsterDeath, eventOptions, SendOptions.SendReliable);
            MessageHub.SendMessage(MessageType.DestroyEnemy, this, _timeBeforeDestroy);
            Animator.speed = 1;
            Animator.SetTrigger("Kill");
            if (_traitDictionary.TryGetValue(typeof(PositionBroadCastOnDeath), out EnemyTrait trait))
                trait.Execute(this);
            NavMeshAgent.enabled = false;
            foreach (var col in GetComponents<Collider>())
            {
                col.enabled = false;
            }
            _threatManager.enabled = false;
            StatusEffectHandler.enabled = false;
            enabled = false;
        }

        private void OnDeathBroadCast(Vector3 position)
        {
            _deathBroadCastPosition = position;
        }
        #endregion

        #region Public Methods
        [PunRPC]
        public void TakeDamage(int amount, int pViewId, PhotonMessageInfo info)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            EnemyManager.Instance.ActivateEnemy(photonView.ViewID, true);

            int damageTaken = (int)(amount * CurrentStats.DamageTakenMultiplier.Value);
            CurrentStats.Health.Reduce(damageTaken);

            if (CurrentStats.Health.Value == 0)
            {
                Die();
            }
            else
            {
                if (GameManager.Instance.PlayerDictionary.TryGetValue(info.Sender, out NetworkPlayer nPlayer))
                {
                    if (!_threatManager.AddPlayer(nPlayer, new PlayerThreatData(_threatManager.ThreatManagerConfig.BaseThreat + damageTaken)))
                    {
                        _threatManager.UpdatePlayer(nPlayer, dmgInFrame: damageTaken);
                    }
                }
                else
                    Debug.LogError($"No such player: {info.Sender}");
            }
        }

        [PunRPC]
        public void StartRoot(float duration)
        {
            CurrentStats.IsRooted.StartRoot(duration);
        }
        #endregion

        #region IPunObservableCallback
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(CurrentStats.Health.Value);
                    stream.SendNext(CurrentStats.DamageTakenMultiplier.Value);
                    stream.SendNext(CurrentStats.IsRooted.Value);
                    stream.SendNext(CurrentStats.MoveSpeed.Value);
                }
            }
            else
            {
                CurrentStats.Health.Value = (int)stream.ReceiveNext();
                CurrentStats.DamageTakenMultiplier.Value = (float)stream.ReceiveNext();
                CurrentStats.IsRooted.Value = (bool)stream.ReceiveNext();
                CurrentStats.MoveSpeed.Value = (float)stream.ReceiveNext();
            }
        }
        #endregion
    }
}