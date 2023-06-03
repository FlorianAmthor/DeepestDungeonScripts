using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects;
using WatStudios.DeepestDungeon.Core.Attributes;
using WatStudios.DeepestDungeon.Core.WeaponLogic;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.UI.Gameplay;

namespace WatStudios.DeepestDungeon.Core.PlayerLogic
{
    [RequireComponent(typeof(Animator), typeof(Collider), typeof(PlayerInputController))]
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkPlayer : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback, IStatusEntity
    {
        #region Private Fields
        private Animator _animator;
        private CapsuleCollider _capsuleCol;
        private Rigidbody _rigidbody;
        private PlayerUI _playerUi;
        private RaycastHit _interactiveRaycastHit;
        private bool[] _scriptWasEnabled;
        private bool _fallStarted;
        private float _initialFallHeight = 0.0f;
        private Vector3 _groundNormal;
        private static int _numOfThingsBlockingInput;
        internal static bool AllowPlayerInput { get => _numOfThingsBlockingInput == 0; }
        private const float _defaultSpeedFactor = 1.0f;

        private Dictionary<FpsState, MoveSpeedFpsStateData> _moveSpeedFpsDictionary;
        #endregion

        internal bool wasTeleported;

        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private Behaviour[] _playerBehaviours;
        [Header("Player Variables")]
        [SerializeField] private PlayerBaseStats _baseStats;
        [SerializeField] private float _jumpPower;
        [SerializeField] private bool _invincible;
        [SerializeField, Range(0.05f, 0.15f)] private float _fallOffset;
        [Header("Components")]
        [SerializeField] private Transform _weaponHolder;
        [SerializeField] private Transform _weaponRig;
        [Header("Camera components")]
        [SerializeField] private Camera _fpsCamera;
        [SerializeField] private Camera _killCam;
        [SerializeField] private float _crouchHeightFactor;
        [SerializeField] private LayerMask _remotePlayerMask;
        [SerializeField] private LayerMask _groundCheckMask;
        [Header("Fall damage values")]
        [SerializeField, Tooltip("This value will be added to the jump height of the player")]
        private float _fallingDistanceHurt;
        [SerializeField, Tooltip("This value will be added to the jump height of the player")]
        private float _fallingDistanceDeath;
        [SerializeField, Range(0, 1), Tooltip("minimal fall damage as percent of max health")]
        private float _minFallDamageInPercent;
        [Header("Move Speed Factors")]
        [SerializeField] private MoveSpeedData _moveSpeedData;
        [Header("UI")]
        [SerializeField, Tooltip("The Player's UI GameObject Prefab")]
        private GameObject _playerUiPrefab;
#pragma warning restore 649
        #endregion

        #region Properties
        public static NetworkPlayer LocalPlayerInstance { get; private set; }
        public Player PhotonPlayer { get; private set; }
        internal PlayerCurrentStats CurrentStats { get; private set; }
        public PlayerBaseStats BaseStats { get => _baseStats; }
        public StatusEffectHandler StatusEffectHandler { get; private set; }
        public PhotonView PhotonView => photonView;
        public Vector3 MoveDirection { get; internal set; }
        public bool IsCrouching { get; internal set; }
        public bool IsJumping { get; internal set; }
        public bool IsWalking { get; internal set; }
        public bool IsMoving { get; internal set; }
        public bool IsGrounded { get; private set; }
        public Camera FpsCamera { get => _fpsCamera; }
        internal bool IsDead { get; private set; }
        internal bool AllowMovementInput { get; set; }
        internal LayerMask RemotePlayerMask { get => _remotePlayerMask; }
        internal Transform WeaponHolder { get => _weaponHolder; }
        internal WeaponSystem WeaponSystem { get; private set; }
        internal PlayerInputController PlayerInputController { get; private set; }
        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            CurrentStats = new PlayerCurrentStats(_baseStats);
            if (photonView.IsMine)
            {
                LocalPlayerInstance = this;
                PhotonPlayer = PhotonNetwork.LocalPlayer;
                InstantiatePlayerUI();
            }
            PhotonPlayer = PhotonNetwork.LocalPlayer.Get(photonView.OwnerActorNr);
            _numOfThingsBlockingInput = 0;

            Init();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            RegisterPlayer();
            MessageHub.SendMessage(MessageType.PlayerHealthChanged, CurrentStats.Health.HealthPercent, photonView.OwnerActorNr);
            if (!photonView.IsMine)
                return;
        }

        private void Update()
        {
            if (!photonView.IsMine)
                return;
            CheckGroundStatus(out _groundNormal);
            if (IsGrounded && GetComponent<Rigidbody>().velocity.magnitude > 0)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                AllowMovementInput = true;
            }
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine)
                return;
            Move();
        }

        private void OnEnable()
        {
            if (!photonView.IsMine)
                return;
            MessageHub.Subscribe(MessageType.PlayerCrouchKeyPressed, OnCrouchingKeyPressed, ActionExecutionScope.Gameplay);
            MessageHub.Subscribe(MessageType.AllowPlayerInput, OnAllowPlayerInput, ActionExecutionScope.Gameplay);
        }

        private void OnDisable()
        {
            if (!photonView.IsMine)
                return;
            MessageHub.Unsubscribe(MessageType.PlayerCrouchKeyPressed, OnCrouchingKeyPressed);
            MessageHub.Unsubscribe(MessageType.AllowPlayerInput, OnAllowPlayerInput);
        }
        #endregion

        #region IPunObservable
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(CurrentStats.Health.Value);
                stream.SendNext(_invincible);
            }
            else
            {
                var currentHealth = (int)stream.ReceiveNext();
                if (currentHealth != CurrentStats.Health.Value)
                {
                    CurrentStats.Health.Value = currentHealth;
                    MessageHub.SendMessage(MessageType.PlayerHealthChanged, CurrentStats.Health.HealthPercent, photonView.OwnerActorNr);
                }
                _invincible = (bool)stream.ReceiveNext();
            }
        }
        #endregion

        #region IPunInstantiateMagicCallback Implementation
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            MessageHub.SendMessage(MessageType.PlayerInstantiate, this, PhotonPlayer);
        }
        #endregion

        #region MessagingCallbacks
        private void OnAllowPlayerInput(Message msg)
        {
            bool data = (bool)msg.Data[0];
            if (data)
                _numOfThingsBlockingInput--;
            else
                _numOfThingsBlockingInput++;

            if (AllowPlayerInput)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;

            Cursor.visible = !AllowPlayerInput;
        }

        /// <summary>
        /// Scales the Capsule Collider and moves the camera down, if the player presses the crouch button
        /// </summary>
        /// <param name="crouch"></param>
        private void OnCrouchingKeyPressed(Message msg)
        {
            HandleCrouching();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Inizialisation and sets everything to Default
        /// </summary>
        private void Init()
        {
            CacheComponents();
            if (photonView.IsMine)
            {
                _moveSpeedFpsDictionary = new Dictionary<FpsState, MoveSpeedFpsStateData>();
                AllowMovementInput = true;
                foreach (var value in _moveSpeedData.moveSpeedWrappers)
                {
                    _moveSpeedFpsDictionary.Add(value.FpsState, value.MoveSpeedFpsStateData);
                }
                _scriptWasEnabled = new bool[_playerBehaviours.Length];
                for (int i = 0; i < _scriptWasEnabled.Length; i++)
                {
                    _scriptWasEnabled[i] = _playerBehaviours[i].enabled;
                }
                SetDefault();
            }
            else
            {
                int layer = (int)Math.Log(_remotePlayerMask.value, 2);
                gameObject.layer = layer;
                foreach (var trans in GetComponentsInChildren<Transform>())
                {
                    if (!trans.gameObject.activeSelf)
                    {
                        trans.gameObject.SetActive(true);
                        trans.gameObject.layer = layer;
                        trans.gameObject.SetActive(false);
                    }
                    else
                        trans.gameObject.layer = layer;
                }
                _fpsCamera.gameObject.SetActive(false);
            }
            WeaponSystem.Init(this);
        }

        /// <summary>
        /// Sets the default if player dies
        /// </summary>
        private void SetDefault()
        {
            IsDead = false;
            GetComponent<Animator>().SetBool("isDead", IsDead);
            CurrentStats = new PlayerCurrentStats(_baseStats);
            _fpsCamera.gameObject.SetActive(true);
            _killCam.gameObject.SetActive(false);
            _weaponHolder.gameObject.SetActive(true);

            for (int i = 0; i < _playerBehaviours.Length; i++)
            {
                _playerBehaviours[i].enabled = _scriptWasEnabled[i];
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = true;
        }

        /// <summary>
        /// Registers the Player by Name
        /// </summary>
        private void RegisterPlayer()
        {
            string ID = PhotonPlayer.NickName + " " + photonView.ViewID;
            transform.name = ID;
        }

        /// <summary>
        /// Player Movement Logic
        /// </summary>
        private void Move()
        {
            float dt = Time.deltaTime;
            float speedFactor = _defaultSpeedFactor;

            if (_moveSpeedFpsDictionary.TryGetValue(PlayerInputController.FpsState, out MoveSpeedFpsStateData moveSpeedStateValue) && WeaponSystem.ActiveWeaponSlot != ActiveWeaponSlot.Unarmed)
            {
                if (WeaponSystem.CurrentWeapon.CurrentStats.Zoom.IsZooming)
                    speedFactor = moveSpeedStateValue.SpeedModifier * moveSpeedStateValue.ZoomSpeedModifier;
                else
                    speedFactor = moveSpeedStateValue.SpeedModifier;
            }
            else
                speedFactor = _defaultSpeedFactor;

            //Used for to calculate values important for animation
            if (!AllowMovementInput)
                MoveDirection = Vector3.zero;

            Vector3 inverseDirection = transform.InverseTransformDirection(MoveDirection);
            inverseDirection = Vector3.ProjectOnPlane(inverseDirection, _groundNormal);
            float forward = inverseDirection.z;
            float sideward = inverseDirection.x;

            float appliedMoveSpeed = speedFactor * CurrentStats.MoveSpeed.Value;
            Vector3 velocity = MoveDirection * appliedMoveSpeed;

            if (IsJumping)
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpPower, _rigidbody.velocity.z);
                IsJumping = false;
            }
            _rigidbody.MovePosition(_rigidbody.position + velocity * dt);
            UpdateAnimator(inverseDirection, speedFactor, forward, sideward);
        }

        private void HandleCrouching()
        {
            if (!IsCrouching)
            {
                _capsuleCol.height *= _crouchHeightFactor;
                _capsuleCol.center *= _crouchHeightFactor;
                _fpsCamera.transform.position = new Vector3(_fpsCamera.transform.position.x, transform.position.y + _capsuleCol.height, _fpsCamera.transform.position.z);
                var weaponRigPos = _weaponRig.transform.position;
                weaponRigPos.y *= _crouchHeightFactor;
                _weaponRig.transform.position = weaponRigPos;
                IsCrouching = true;
            }
            else if (IsCrouching)
            {
                float originalHeight = _capsuleCol.height / _crouchHeightFactor;
                Vector3 p1 = transform.position + Vector3.up * (originalHeight - _capsuleCol.radius + Physics.defaultContactOffset);
                if (Physics.SphereCast(p1, _capsuleCol.radius, Vector3.up, out _, 1, _groundCheckMask.value))
                    return;
                _capsuleCol.height /= _crouchHeightFactor;
                _capsuleCol.center /= _crouchHeightFactor;
                _fpsCamera.transform.position = new Vector3(_fpsCamera.transform.position.x, transform.position.y + originalHeight, _fpsCamera.transform.position.z);
                var weaponRigPos = _weaponRig.transform.position;
                weaponRigPos.y /= _crouchHeightFactor;
                _weaponRig.transform.position = weaponRigPos;
                IsCrouching = false;
            }
            _animator.SetBool("Crouch", IsCrouching);
        }

        /// <summary>
        /// Checks if we are touching the ground or falling
        /// </summary>
        private void CheckGroundStatus(out Vector3 groundNormal)
        {
            Vector3 p1 = transform.position + _capsuleCol.center + Vector3.up * (-_capsuleCol.height * 0.5f + _capsuleCol.radius + _fallOffset - Physics.defaultContactOffset);
            if (Physics.SphereCast(p1, _capsuleCol.radius, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, _groundCheckMask.value))
            {
                groundNormal = hitInfo.normal;
                float fallDistance = hitInfo.distance;
                if (fallDistance > _fallOffset)
                {
                    IsGrounded = false;

                    if (!_fallStarted)
                    {
                        _initialFallHeight = transform.position.y;
                        _fallStarted = true;
                    }
                }
                else
                {
                    IsGrounded = true;
                    if (_fallStarted)
                    {
                        float heightDifference = Mathf.Abs(_initialFallHeight - transform.position.y);

                        if (_fallingDistanceHurt <= heightDifference + _fallOffset && !wasTeleported)
                            ApplyFallDamage(heightDifference);
                    }
                    if (wasTeleported)
                        wasTeleported = false;
                    _initialFallHeight = transform.position.y;
                    _fallStarted = false;
                }
                _animator.SetBool("Grounded", IsGrounded);
            }
            else
            {
                groundNormal = Vector3.zero;
            }
        }

        /// <summary>
        /// Updates the animations values of the animator component
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="speed"></param>
        /// <param name="forwardAmount">Used for Inverse Kinematics</param>
        /// <param name="sideAmount">Used for Inverse Kinematics</param>
        private void UpdateAnimator(Vector3 velocity, float speedFactor, float forwardAmount, float sideAmount)
        {
            _animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            _animator.SetFloat("Side", sideAmount, 0.1f, Time.deltaTime);
            _animator.SetBool("Moving", IsMoving);

            if (IsGrounded && IsMoving)
            {
                _animator.speed = speedFactor;
            }
            else if (!IsGrounded && IsMoving || !IsMoving)
            {
                _animator.speed = 1;
            }
        }

        /// <summary>
        /// Calculates fall damage, more height corresponds to more fall damage
        /// </summary>
        private void ApplyFallDamage(float fallHeight)
        {
            if (fallHeight >= _fallingDistanceDeath)
            {
                photonView.RPC("TakeDamage", RpcTarget.All, int.MaxValue, photonView.ViewID);
            }
            else
            {
                float newIntervalLength = _fallingDistanceDeath - _fallingDistanceHurt;
                float bonusDamageHeight = fallHeight - _fallingDistanceHurt;
                float intervallFactor = bonusDamageHeight * (newIntervalLength / 100f);
                float fallDamageFactor = _minFallDamageInPercent + (1f - _minFallDamageInPercent) * intervallFactor;
                photonView.RPC("TakeDamage", RpcTarget.All, (int)(fallDamageFactor * CurrentStats.Health.MaxValue), photonView.ViewID);
            }
        }

        /// <summary>
        /// Checks for null components and caches them in a local variable
        /// </summary>
        private void CacheComponents()
        {
            _animator = GetComponent<Animator>();
            _capsuleCol = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            StatusEffectHandler = GetComponent<StatusEffectHandler>();
            WeaponSystem = GetComponent<WeaponSystem>();
            PlayerInputController = GetComponent<PlayerInputController>();
        }

        /// <summary>
        /// Instantiates the player UI and sets its target to the local gameobject
        /// </summary>
        private void InstantiatePlayerUI()
        {
            if (_playerUiPrefab != null && photonView.IsMine)
            {
                GameObject _uiGo = Instantiate(_playerUiPrefab);
                _uiGo.transform.SetParent(gameObject.transform);
                _playerUi = _uiGo.GetComponent<PlayerUI>();
                _playerUi.SetTarget(gameObject);
            }
            else
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
        /// <summary>
        /// Handles the Dying of the player and starts the respawn timer
        /// </summary>
        private void Die()
        {
            IsDead = true;
            _killCam.gameObject.SetActive(true);
            _fpsCamera.gameObject.SetActive(false);
            //Disable CurrentWeapon, or unequip
            _weaponHolder.gameObject.SetActive(false);
            _rigidbody.velocity = Vector3.zero;

            GetComponent<Animator>().SetBool("isDead", IsDead);

            for (int i = 0; i < _playerBehaviours.Length; i++)
            {
                _playerBehaviours[i].enabled = false;
            }

            StartCoroutine(Respawn());
        }

        /// <summary>
        /// Adds the given <paramref name="amount"/> to the current health of the local player
        /// </summary>
        /// <param name="amount"></param>
        [PunRPC]
        public void RecieveHealing(int amount)
        {
            if (IsDead || !photonView.IsMine)
                return;
            CurrentStats.Health.Increase(amount * CurrentStats.HealEffectivity.Value);
            MessageHub.SendMessage(MessageType.PlayerHealthChanged, CurrentStats.Health.HealthPercent, photonView.OwnerActorNr);
        }

        /// <summary>
        /// Handles the Damage taking
        /// </summary>
        /// <param name="amount"></param>
        [PunRPC]
        public void TakeDamage(int amount, int pViewId, PhotonMessageInfo info)
        {
            if (IsDead || _invincible || !photonView.IsMine)
                return;

            if (pViewId != photonView.ViewID)
            {
                int damageAfterShield;

                if (StatusEffectHandler.TryGetStatusEffect(typeof(ThornArmorEffect), out ThornArmorEffect statuseffect))
                {
                    PhotonView pView = PhotonView.Find(pViewId);
                    var entity = pView.GetComponent<IStatusEntity>();
                    //TODO: Check if working
                    if (entity != null)
                        pView.RPC("TakeDamage", RpcTarget.MasterClient, statuseffect.AmountReflected, photonView.ViewID);
                }

                // Uncomment this line if we want reflected damage to be nullified for us
                // amount -= statuseffect.AmountReflected;

                damageAfterShield = CurrentStats.ShieldPoints.Reduce(amount);

                if (damageAfterShield > 0)
                {
                    CurrentStats.Health.Reduce(damageAfterShield * CurrentStats.DamageTakenMultiplier.Value);
                }
            }
            else
            {
                CurrentStats.Health.Reduce(amount);
            }

            //GetComponentInChildren<ScreenOverlay>().FadeOverlay(); // TODO Für Flo zur implementierung wie du es gerne hättest. 
            //GetComponentInChildren<ScreenOverlay>().UpdateOverlayAnim(CurrentStats.Health.HealthPercent); // TODO Für Flo zur implementierung wie du es gerne hättest. Hier muss ein prozentwert von 0 bis 1 weitergegeben werden.

            MessageHub.SendMessage(MessageType.PlayerHealthChanged, CurrentStats.Health.HealthPercent, photonView.OwnerActorNr);

            if (CurrentStats.Health.Value == 0 && photonView.IsMine)
                Die();
        }

        /// <summary>
        /// The respawn timer function
        /// </summary>
        /// <returns></returns>
        private IEnumerator Respawn()
        {
            MessageHub.SendMessage(MessageType.DissolveRoutine);
            yield return new WaitForSeconds(GameManager.Instance.RespawnTime);

            SetDefault();
            MessageHub.SendMessage(MessageType.PlayerHealthChanged, CurrentStats.Health.HealthPercent, photonView.OwnerActorNr);
            //TODO: Spawnposition for gameplaye scene, damage is disabled in lobby
            transform.position = GameManager.Instance.GameplaySpawnPosition;
            transform.rotation = Quaternion.identity;
            MessageHub.SendMessage(MessageType.DissolveRoutine);
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Handels the Assigning of the Layers and Children Layers for the Differen Cameras
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layerName"></param>
        internal void AssignLayer(GameObject obj, string layerName)
        {
            Transform[] Children = obj.GetComponentsInChildren<Transform>();
            obj.layer = LayerMask.NameToLayer(layerName);
            foreach (Transform child in Children)
                child.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
        #endregion
    }
}
