using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Utility;

using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic
{
    public class EnemyManager : MonoBehaviourPun
    {
        #region Singleton
        public static EnemyManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
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
        [SerializeField] private GameObject _bossPrefab;
        [SerializeField] private float _activeDistance;
        [SerializeField] private float _updatesPerSecond;
#pragma warning restore 649
        #endregion

        #region Private Fields
        /// <summary>
        /// List of Objects that get destroyed next frame
        /// </summary>
        private TimedObjectDestroyer<EnemyEntity> _objDestroyer;
        private Dictionary<int, EnemyEntity> _entities = new Dictionary<int, EnemyEntity>();
        private Dictionary<int, EnemyEntity> _activeEntities = new Dictionary<int, EnemyEntity>();
        private Dictionary<int, EnemyEntity> _passiveEntities = new Dictionary<int, EnemyEntity>();
        private List<EnemyEntity> _activeEntityList;
        private List<EnemyEntity> _scheduledEnemies;
        private int _killedMonsters;
        private float _fixedTimeStep;
        private float _physicsFPS;
        private float _updateInterval;
        #endregion

        #region Properties
        public Dictionary<int, EnemyEntity> Entities => _entities;
        public Dictionary<int, EnemyEntity> ActiveEntities => _activeEntities;
        public Dictionary<int, EnemyEntity> _assiveEntities => _passiveEntities;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            _objDestroyer = new TimedObjectDestroyer<EnemyEntity>();
            _activeEntityList = new List<EnemyEntity>();
            ActivateEnemies();
            _fixedTimeStep = Time.fixedDeltaTime;
            _physicsFPS = 1.0f / _fixedTimeStep;
            _updateInterval = _physicsFPS / _updatesPerSecond;
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            _objDestroyer.Update(Time.deltaTime);
            ActivateEnemies();
        }

        private void ActivateEnemies()
        {
            var passiveEntities = new List<EnemyEntity>(_passiveEntities.Values);
            var players = new List<NetworkPlayer>(GameManager.Instance.PlayerDictionary.Values);
            foreach (var entity in passiveEntities)
            {
                foreach (var player in players)
                {
                    if (Vector3.Distance(entity.transform.position, player.transform.position) <= _activeDistance)
                    {
                        ActivateEnemy(entity.photonView.ViewID, true);
                        break;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (_activeEntities.Count == 0)
                return;
            ScheduleEnemiesForFrame();

            foreach (var enemyEntity in _scheduledEnemies)
            {
                enemyEntity.Tick();
            }
        }

        private void LateUpdate()
        {
            if (_activeEntities.Count == 0)
                return;
            if (_scheduledEnemies == null || _scheduledEnemies?.Count == 0)
                return;
            foreach (var enemyEntity in _scheduledEnemies)
            {
                enemyEntity.StatusEffectHandler.LateTick();
            }
        }

        private void OnEnable()
        {
            if (this != Instance)
                return;
            MessageHub.Subscribe(MessageType.DestroyEnemy, OnDestroyEnemy, ActionExecutionScope.Gameplay);
            MessageHub.Subscribe(MessageType.EnemyStatusChange, OnEnemyStatusChanged, ActionExecutionScope.Gameplay);
        }

        private void OnDisable()
        {
            if (this != Instance)
                return;
            MessageHub.Unsubscribe(MessageType.DestroyEnemy, OnDestroyEnemy);
            MessageHub.Unsubscribe(MessageType.EnemyStatusChange, OnEnemyStatusChanged);

        }
        #endregion

        #region Messaging Callbacks
        private void OnEnemyStatusChanged(Message obj)
        {
            int pViewID = (int)obj.Data[0];
            bool isEnabled = (bool)obj.Data[1];

            ActivateEnemy(pViewID, isEnabled);
        }

        private void OnDestroyEnemy(Message obj)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            EnemyEntity entity = obj.Data[0] as EnemyEntity;
            _entities.Remove(entity.PhotonView.ViewID);
            if (!_activeEntities.Remove(entity.PhotonView.ViewID))
                _passiveEntities.Remove(entity.PhotonView.ViewID);
            else
                _activeEntityList.Remove(entity);
            _objDestroyer.Add(entity, timeBeforeDestroy: (float)obj.Data[1]);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Spawns a boss Monster in the middle of the map
        /// </summary>
        private void SpawnBoss(Vector3 pos, int radius)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            CleanUp();

            _entities.Clear();
            _activeEntities.Clear();
            _passiveEntities.Clear();

            var bossObj = PhotonNetwork.InstantiateSceneObject(_bossPrefab.name, pos, Quaternion.identity);
            var bossEntity = bossObj.GetComponent<EnemyEntity>();
            _entities.Add(bossObj.GetPhotonView().ViewID, bossEntity);
        }

        internal void CleanUp()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            while (_entities.Count != 0)
            {
                var entityPair = _entities.First();
                _entities.Remove(entityPair.Key);
                PhotonNetwork.Destroy(entityPair.Value.gameObject);
            }
        }

        internal bool IsEnemyActive(EnemyEntity enemyEntity)
        {
            return _activeEntities.ContainsValue(enemyEntity);
        }

        internal void ActivateEnemy(int pViewID, bool isEnabled)
        {
            if (isEnabled)
            {
                if (_activeEntities.ContainsKey(pViewID))
                    return;
                if (_entities.TryGetValue(pViewID, out EnemyEntity entity))
                {
                    _passiveEntities.Remove(pViewID);
                    if (!_activeEntities.ContainsKey(pViewID))
                    {
                        entity.Animator.enabled = true;
                        _activeEntities.Add(pViewID, entity);
                        _activeEntityList.Add(entity);
                    }
                }
            }
            else
            {
                if (_passiveEntities.ContainsKey(pViewID))
                    return;
                if (_entities.TryGetValue(pViewID, out EnemyEntity entity))
                {
                    _activeEntities.Remove(pViewID);
                    if (!_passiveEntities.ContainsKey(pViewID))
                    {
                        entity.Animator.enabled = false;
                        _passiveEntities.Add(pViewID, entity);
                        _activeEntityList.Remove(entity);
                    }
                }
            }
        }

        private void ScheduleEnemiesForFrame()
        {
            int numOfEnemiesToUpdate = (int)(_activeEntities.Count / _updateInterval);
            if (numOfEnemiesToUpdate <= 0)
                numOfEnemiesToUpdate = 1;
            _scheduledEnemies = new List<EnemyEntity>(numOfEnemiesToUpdate);

            for (int i = 0; i < numOfEnemiesToUpdate; i++)
            {
                var enemy = _activeEntityList[0];
                _activeEntityList.Remove(enemy);
                _activeEntityList.Add(enemy);
                _scheduledEnemies.Add(enemy);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Spawns the enmy at given given pos.
        /// </summary>
        /// <param name="pos">Position to spawn at</param>
        /// <param name="enemyPrefab">Name of the enemy</param>
        /// <returns>Returns if the enemy was created successfully</returns>
        public bool SpawnEnemy(Vector3 pos, string enemyName)
        {
            if (!PhotonNetwork.IsMasterClient)
                return false;
            try
            {
                var enemyObj = PhotonNetwork.InstantiateSceneObject("Enemies/" + enemyName, pos, Quaternion.identity);
                enemyObj.GetComponent<NavMeshAgent>().Warp(pos);
                var enemyEntity = enemyObj.GetComponent<EnemyEntity>();
                _entities.Add(enemyEntity.photonView.ViewID, enemyEntity);
                ActivateEnemy(enemyEntity.photonView.ViewID, false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Spawns the enmy at given given pos.
        /// </summary>
        /// <param name="pos">Position to spawn at</param>
        /// <param name="enemyPrefab">Enemy to spawn</param>
        /// <returns>Returns if the enemy was created successfully</returns>
        public bool SpawnEnemy(Vector3 pos, GameObject enemyPrefab)
        {
            if (!PhotonNetwork.IsMasterClient)
                return false;
            try
            {
                var enemyObj = PhotonNetwork.InstantiateSceneObject("Enemies/" + enemyPrefab.name, pos, Quaternion.identity);
                enemyObj.GetComponent<NavMeshAgent>().Warp(pos);
                var enemyEntity = enemyObj.GetComponent<EnemyEntity>();
                _entities.Add(enemyEntity.photonView.ViewID, enemyEntity);
                ActivateEnemy(enemyEntity.photonView.ViewID, false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    #endregion
}