using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.PlayerLogic;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Utility.CustomPhoton;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;
using Random = UnityEngine.Random;

namespace WatStudios.DeepestDungeon.Core.WeaponLogic
{
    public class WeaponSystem : MonoBehaviourPun, IPunObservable
    {
        #region Private Fields
        private NetworkPlayer _networkPlayer;
        private PlayerInputController _playerInputController;
        private Animator _animator;
        private PhotonIKView _photonIKView;
        private bool _isAiming;
        private float _cameraView;
        private float _weaponView;
        private float _zoomingTimer;
        private float _amingTimer;
        private Coroutine _reloadCoroutine;
        private Dictionary<FpsState, SpreadFpsStateData> _spreadFpsDictionary;
        private Dictionary<ActiveWeaponSlot, GameObject> _weaponObjects;



        private Quaternion _rotNeck;
        private Quaternion _rotUpperChest;
        private Quaternion _rotChest;
        #endregion

        #region Internal
        internal bool HasWeapon { get; private set; }
        internal bool IsFiring { get; set; }
        internal ActiveWeaponSlot PreviousActiveWeaponSlot { get; private set; }
        internal ActiveWeaponSlot ActiveWeaponSlot { get; private set; }
        internal Weapon CurrentWeapon { get => _weaponObjects[ActiveWeaponSlot].GetComponent<Weapon>(); }
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [Header("Layers")]
        //[SerializeField] private LayerMask _localLayerMask;
        [SerializeField] private LayerMask _weaponLayerMask;
        [SerializeField] private LayerMask _damageableLayerMask;

        [Header("FPS State Spread Values")]
        [SerializeField] private bool _useSpreadSystem;
        [SerializeField] private List<MoveSpeedFpsStateWrapper> _spreadFPSStates;

        [Header("WeaponInUse")]
        [SerializeField] private bool _weaponModeAsDefault;
        [SerializeField] private LayerMask _shootingLayerMask;
        [SerializeField] private GameObject[] _availableWeapons;
#pragma warning restore 649
        #endregion

        #region MonoBehaviour Callbacks
        /// <summary>
        /// Does all the Button Handeling and the Animator switch 
        /// from no Weapon to Weapon 
        /// </summary>
        private void Update()
        {
            if (!photonView.IsMine)
            {
                //foreach(Transform child in transform)
                //{
                //    if (child.tag == "LocalPlayer")
                //        child.gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
                //}

                //GameObject[] LocalPlayers = GameObject.FindGameObjectsWithTag("LocalPlayer");
                //foreach (GameObject player in LocalPlayers)
                //{
                //    player.layer = (int)Math.Log(_weaponLayerMask.value, 2);
                //}
                return;
            }

            if (HasWeapon)
            {
                if (_useSpreadSystem)
                    SpreadHandler();
            }
            HandleOverheat();

            if (!NetworkPlayer.AllowPlayerInput)
                return;
            float dt = Time.deltaTime;
            float currentTime = Time.time;

            if (HasWeapon)
            {
                _networkPlayer.WeaponHolder.rotation = _networkPlayer.FpsCamera.transform.rotation;

                if (CurrentWeapon.BaseStats.AmmoType == AmmoType.Overheat && !CurrentWeapon.CurrentStats.OverHeat.IsCoolingDown)
                {
                    if (CurrentWeapon.CurrentStats.OverHeat.Value >= 1.0f)
                    {
                        CurrentWeapon.CurrentStats.OverHeat.IsCoolingDown = true;
                        CurrentWeapon.IsShooting = false;
                        MessageHub.SendMessage(MessageType.OverheatCooldown, true);
                        StartCoroutine(CooldownPenaltyRoutine(CurrentWeapon));
                    }
                    else
                    {
                        if (Input.GetButton("Fire1"))
                        {
                            if (CurrentWeapon.CurrentStats.LastFired < currentTime - 1.0f / CurrentWeapon.CurrentStats.AttackSpeed.Value)
                            {
                                Shoot();
                                CurrentWeapon.CurrentStats.LastFired = currentTime;
                            }
                        }
                    }
                }
                else if (CurrentWeapon.BaseStats.AmmoType == AmmoType.Magazine)
                {
                    if (CurrentWeapon.CurrentStats.Magazine.Value > 0)
                    {
                        if (CurrentWeapon.BaseStats.FiringMode == FiringMode.Automatic)
                        {
                            if (CurrentWeapon.CurrentStats.Magazine.IsReloading == false)
                            {
                                if (Input.GetButton("Fire1"))
                                {
                                    if (CurrentWeapon.CurrentStats.LastFired < currentTime - 1.0f / CurrentWeapon.CurrentStats.AttackSpeed.Value)
                                    {
                                        Shoot();
                                        CurrentWeapon.CurrentStats.LastFired = currentTime;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CurrentWeapon.CurrentStats.Magazine.IsReloading == false)
                            {
                                if (Input.GetButtonDown("Fire1") && Input.GetButton("Fire1"))
                                {
                                    if (CurrentWeapon.CurrentStats.LastFired < currentTime - 1.0f / CurrentWeapon.CurrentStats.AttackSpeed.Value)
                                    {
                                        Shoot();
                                        CurrentWeapon.CurrentStats.LastFired = currentTime;
                                    }
                                }
                                else
                                    CurrentWeapon.IsShooting = false;
                            }
                        }
                    }
                }

                // Cancel Effects // Needed Else Bug
                if (Input.GetButtonUp("Fire1") || CurrentWeapon.CurrentStats.Magazine.Value <= 0)
                {
                    CurrentWeapon.IsShooting = false;
                    MessageHub.SendMessage(MessageType.PlayWeaponSound, CurrentWeapon.IsShooting);
                    _weaponObjects[ActiveWeaponSlot].GetComponent<Animator>().SetBool("Fire", false);
                }

                if (Input.GetButtonDown("Fire2"))
                {
                    _zoomingTimer = _amingTimer = 0;
                    CurrentWeapon.CurrentStats.Zoom.SetZooming(CurrentWeapon.CurrentStats.Zoom.CanZoom);
                    _isAiming = true;
                }
                else if (Input.GetButtonUp("Fire2"))
                {
                    _zoomingTimer = _amingTimer = 0;
                    CurrentWeapon.CurrentStats.Zoom.SetZooming(false);
                    _isAiming = false;
                }

                if (Input.GetButtonDown("Reload"))
                {
                    if (CurrentWeapon.CurrentStats.Magazine.Value < CurrentWeapon.CurrentStats.Magazine.MaxValue && !CurrentWeapon.CurrentStats.Magazine.IsReloading)
                    {
                        CurrentWeapon.GetComponent<Animator>().SetTrigger("Reload");
                        CurrentWeapon.CurrentStats.Magazine.IsReloading = true;
                        _reloadCoroutine = StartCoroutine(Reload());
                    }
                }

                if (_isAiming)
                {
                    if (CurrentWeapon.AimingHolder.localPosition != CurrentWeapon.AimingPos || CurrentWeapon.AimingHolder.localRotation != CurrentWeapon.AimingRot)
                    {
                        CurrentWeapon.AimingHolder.localPosition = Vector3.Lerp(CurrentWeapon.AimingHolder.localPosition, CurrentWeapon.AimingPos, _amingTimer);
                        CurrentWeapon.AimingHolder.localRotation = Quaternion.Lerp(CurrentWeapon.AimingHolder.localRotation, CurrentWeapon.AimingRot, _amingTimer);
                        _amingTimer += dt;
                    }
                }
                else
                {
                    if (CurrentWeapon.AimingHolder.localPosition != Vector3.zero || CurrentWeapon.AimingHolder.localRotation != Quaternion.identity)
                    {
                        CurrentWeapon.AimingHolder.localPosition = Vector3.Lerp(CurrentWeapon.AimingHolder.localPosition, Vector3.zero, _amingTimer);
                        CurrentWeapon.AimingHolder.localRotation = Quaternion.Lerp(CurrentWeapon.AimingHolder.localRotation, Quaternion.identity, _amingTimer);
                        _amingTimer += dt;
                    }
                }

                if (CurrentWeapon.CurrentStats.Zoom.IsZooming)
                {
                    if (_networkPlayer.FpsCamera.fieldOfView != _cameraView - CurrentWeapon.CurrentStats.Zoom.Factor)
                    {
                        _networkPlayer.FpsCamera.fieldOfView = Mathf.Lerp(_networkPlayer.FpsCamera.fieldOfView, _cameraView - CurrentWeapon.CurrentStats.Zoom.Factor, _zoomingTimer);
                        CurrentWeapon.Camera.fieldOfView = Mathf.Lerp(CurrentWeapon.Camera.fieldOfView, _weaponView - (CurrentWeapon.CurrentStats.Zoom.Factor / 2), _zoomingTimer);
                        _zoomingTimer += dt;
                    }
                }
                else
                {
                    if (_networkPlayer.FpsCamera.fieldOfView != _cameraView)
                    {
                        _networkPlayer.FpsCamera.fieldOfView = Mathf.Lerp(_networkPlayer.FpsCamera.fieldOfView, _cameraView, _zoomingTimer);
                        CurrentWeapon.Camera.fieldOfView = Mathf.Lerp(CurrentWeapon.Camera.fieldOfView, _weaponView, _zoomingTimer);
                        _zoomingTimer += dt;
                    }
                }

                SetNetworkIK();
            }

            if (Input.GetButtonDown("WeaponSwitch"))
            {
                if(ActiveWeaponSlot == ActiveWeaponSlot.First)
                    HandleWeapon(ActiveWeaponSlot.Second);
                else
                    HandleWeapon(ActiveWeaponSlot.First);
            }
                
            if (Input.GetButtonDown("Unarmed"))
                HandleWeapon(ActiveWeaponSlot.Unarmed);

            _animator.SetBool("WeaponOut", HasWeapon);
        }

        /// <summary>
        /// Transforms the Position of the Hands to the pos of the weapon
        /// </summary>
        /// <param name="layerIndex"></param>
        private void OnAnimatorIK(int layerIndex)
        {
            if (!photonView.IsMine) return;

            if (HasWeapon)
            {
                if (CurrentWeapon.Lefthand != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, CurrentWeapon.Lefthand.position);
                    _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                    _animator.SetIKRotation(AvatarIKGoal.LeftHand, CurrentWeapon.Lefthand.rotation);
                }

                if (CurrentWeapon.RightHand != null)
                {
                    _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, CurrentWeapon.RightHand.position);
                    _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, CurrentWeapon.RightHand.rotation);
                }

                float X = _networkPlayer.FpsCamera.transform.localEulerAngles.x;
                if (X > 180) X -= 360;

                if (_animator.GetBoneTransform(HumanBodyBones.Neck))
                {
                    Quaternion rotNeck = _animator.GetBoneTransform(HumanBodyBones.Neck).localRotation;
                    _rotNeck = Quaternion.Euler(rotNeck.eulerAngles.x + X * 0.15f, rotNeck.eulerAngles.y, rotNeck.eulerAngles.z);
                    _animator.SetBoneLocalRotation(HumanBodyBones.Neck, _rotNeck);
                }

                if (_animator.GetBoneTransform(HumanBodyBones.UpperChest))
                {
                    Quaternion rotUpperChest = _animator.GetBoneTransform(HumanBodyBones.UpperChest).localRotation;
                    _rotUpperChest = Quaternion.Euler(rotUpperChest.eulerAngles.x + X * 0.15f, rotUpperChest.eulerAngles.y, rotUpperChest.eulerAngles.z);
                    _animator.SetBoneLocalRotation(HumanBodyBones.UpperChest, _rotUpperChest);
                }

                if (_animator.GetBoneTransform(HumanBodyBones.Chest))
                {
                    Quaternion rotChest = _animator.GetBoneTransform(HumanBodyBones.Chest).localRotation;
                    _rotChest = Quaternion.Euler(rotChest.eulerAngles.x + X * 0.15f, rotChest.eulerAngles.y, rotChest.eulerAngles.z);
                    _animator.SetBoneLocalRotation(HumanBodyBones.Chest, _rotChest);
                }
            }
        }

        private void SetNetworkIK()
        {
            if (CurrentWeapon.RightHand != null)
                _photonIKView.SetRightHandPosition(CurrentWeapon.RightHand.position, CurrentWeapon.RightHand.rotation);
            else
                _photonIKView.DisableRightHand();

            if (CurrentWeapon.Lefthand != null)
                _photonIKView.SetLeftHandPosition(CurrentWeapon.Lefthand.position, CurrentWeapon.Lefthand.rotation);
            else
                _photonIKView.DisableLeftHand();

            _photonIKView.RotateNeck(_rotNeck);
            _photonIKView.RotateUpperchest(_rotUpperChest);
            _photonIKView.RotateChest(_rotChest);

        }
        #endregion

        #region PunObservable Implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(ActiveWeaponSlot);
                stream.SendNext(PreviousActiveWeaponSlot);
            }
            else
            {
                var activeSlot = (ActiveWeaponSlot)stream.ReceiveNext();
                var prevActiveSlot = (ActiveWeaponSlot)stream.ReceiveNext();
                if (ActiveWeaponSlot != activeSlot)
                {
                    ActiveWeaponSlot = activeSlot;
                    PreviousActiveWeaponSlot = prevActiveSlot;
                    _weaponObjects[ActiveWeaponSlot]?.SetActive(true);
                    _weaponObjects[PreviousActiveWeaponSlot]?.SetActive(false);
                }
            }
        }
        #endregion

        #region Private Methods      
        /// <summary>
        /// Inizialisation and sets everything to Default
        /// </summary>
        internal void Init(NetworkPlayer netPlayer)
        {
            _networkPlayer = netPlayer;
            _weaponObjects = new Dictionary<ActiveWeaponSlot, GameObject>(Enum.GetValues(typeof(ActiveWeaponSlot)).Length);
            for (int i = 0; i < Enum.GetValues(typeof(ActiveWeaponSlot)).Length; i++)
            {
                _weaponObjects.Add((ActiveWeaponSlot)i, null);
            }
            if (photonView.IsMine)
            {
                _animator = GetComponent<Animator>();
                _photonIKView = GetComponent<PhotonIKView>();
                _playerInputController = GetComponent<PlayerInputController>();

                for (int i = 0; i < _availableWeapons.Length; i++)
                {
                    _weaponObjects[(ActiveWeaponSlot)i] = _availableWeapons[i];
                    _weaponObjects[(ActiveWeaponSlot)i].GetComponent<Weapon>().Init();
                    _weaponObjects[(ActiveWeaponSlot)i].SetActive(false);

                    int layer = (int)Math.Log(_weaponLayerMask.value, 2);
                    _weaponObjects[(ActiveWeaponSlot)i].layer = layer;
                    Transform[] children = _weaponObjects[(ActiveWeaponSlot)i].GetComponentsInChildren<Transform>();
                    
                    foreach (Transform child in children)
                    {
                        child.gameObject.layer = layer;
                    }
                }

                if (_useSpreadSystem)
                {
                    _spreadFpsDictionary = new Dictionary<FpsState, SpreadFpsStateData>();
                    foreach (var value in _spreadFPSStates)
                    {
                        _spreadFpsDictionary.Add(value.FpsState, value.SpreadFpsStateData);
                    }
                }

                HasWeapon = _weaponModeAsDefault;

                if (HasWeapon)
                {
                    ActiveWeaponSlot = ActiveWeaponSlot.Unarmed;
                    HandleWeapon(ActiveWeaponSlot.First);
                }
                else
                {
                    ActiveWeaponSlot = ActiveWeaponSlot.First;
                    HandleWeapon(ActiveWeaponSlot.Unarmed);
                }


                _weaponObjects[ActiveWeaponSlot].layer = (int)Math.Log(_weaponLayerMask.value, 2);
                _cameraView = _networkPlayer.FpsCamera.fieldOfView;
                _weaponView = CurrentWeapon.Camera.fieldOfView;
                IsFiring = false;
            }
        }

        internal void RegisterRemotePlayerWeapon(Weapon weapon)
        {
            for (int i = 0; i < _availableWeapons.Length; i++)
            {
                if (_availableWeapons[i].GetComponent<Weapon>().BaseStats.Name == weapon.BaseStats.Name)
                {
                    _weaponObjects[(ActiveWeaponSlot)i] = weapon.gameObject;
                    _weaponObjects[(ActiveWeaponSlot)i].gameObject.SetActive((ActiveWeaponSlot)i == ActiveWeaponSlot);
                    break;
                }
            }
        }

        /// <summary>
        /// Applies spread to the weapon according to the weapon stats and the current movement and zooming state
        /// </summary>
        private void SpreadHandler()
        {
            if (!_useSpreadSystem || !HasWeapon)
                return;
            var originalSpread = CurrentWeapon.CurrentStats.Spread.Value;
            _spreadFpsDictionary.TryGetValue(_playerInputController.FpsState, out SpreadFpsStateData spreadStateValue);

            if (_playerInputController.PrevFpsState != _playerInputController.FpsState)
            {
                CurrentWeapon.CurrentStats.Spread.MaxValue = spreadStateValue.SpreadMaxOffset * CurrentWeapon.BaseStats.Spread.MaxValue;
                CurrentWeapon.CurrentStats.Spread.MinValue = spreadStateValue.SpreadMinOffset * CurrentWeapon.CurrentStats.Spread.MaxValue;
                CurrentWeapon.CurrentStats.Spread.Factor = spreadStateValue.SpreadFactorAlteration * CurrentWeapon.BaseStats.Spread.Factor;
            }

            float maxSpread = CurrentWeapon.CurrentStats.Spread.MaxValue;
            float minSpread = CurrentWeapon.CurrentStats.Spread.MinValue;
            float curSpread = CurrentWeapon.CurrentStats.Spread.Value;

            if (CurrentWeapon.IsShooting)
            {
                float addedSpread = (CurrentWeapon.CurrentStats.Zoom.IsZooming ? CurrentWeapon.CurrentStats.Zoom.SpreadReduce * CurrentWeapon.CurrentStats.Spread.Factor : CurrentWeapon.CurrentStats.Spread.Factor);
                CurrentWeapon.CurrentStats.Spread.Value = Mathf.Clamp(curSpread + addedSpread, minSpread, maxSpread);
            }
            else
            {
                if (originalSpread < minSpread)
                    CurrentWeapon.CurrentStats.Spread.Value = Mathf.Lerp(curSpread, minSpread, minSpread);
                else
                    CurrentWeapon.CurrentStats.Spread.Value = Mathf.Clamp(curSpread - CurrentWeapon.CurrentStats.Spread.RegenerationRate, minSpread, maxSpread);
            }
            if (originalSpread != CurrentWeapon.CurrentStats.Spread.Value)
                MessageHub.SendMessage(MessageType.SpreadChanged, CurrentWeapon.CurrentStats.Spread.Value);
        }

        private void HandleOverheat()
        {
            float dT = Time.deltaTime;
            foreach (var weaponObj in _weaponObjects.Values)
            {
                if (weaponObj == null)
                    return;
                var weapon = weaponObj.GetComponent<Weapon>();

                if (weapon.BaseStats.AmmoType != AmmoType.Overheat)
                    return;

                var originalOverheat = weapon.CurrentStats.OverHeat.Value;

                if (weapon.IsShooting)
                {
                    CurrentWeapon.CurrentStats.OverHeat.Increase(dT);
                    MessageHub.SendMessage(MessageType.OverheatChanged, CurrentWeapon.CurrentStats.OverHeat.Value);
                }
                else if (!weapon.IsShooting && weapon.CurrentStats.OverHeat.Value > 0)
                {
                    if (!weapon.CurrentStats.OverHeat.IsCoolingDown)
                        weapon.CurrentStats.OverHeat.Reduce(dT);
                }
                if (originalOverheat != weapon.CurrentStats.OverHeat.Value)
                {
                    MessageHub.SendMessage(MessageType.OverheatChanged, weapon.CurrentStats.OverHeat.Value);
                }
            }
        }

        internal void HandleWeapon(ActiveWeaponSlot newActiveWeaponSlot)
        {
            if (newActiveWeaponSlot == ActiveWeaponSlot && newActiveWeaponSlot != ActiveWeaponSlot.Unarmed)
                return;

            try
            {
                if (CurrentWeapon.BaseStats.AmmoType == AmmoType.Magazine)
                {
                    StopCoroutine(_reloadCoroutine);
                    CurrentWeapon.CurrentStats.Magazine.IsReloading = false;
                }
            }
            catch (Exception) { }

            if (ActiveWeaponSlot != ActiveWeaponSlot.Unarmed)
                CurrentWeapon.IsShooting = false;

            if (newActiveWeaponSlot == ActiveWeaponSlot.Unarmed && ActiveWeaponSlot == ActiveWeaponSlot.Unarmed)
            {
                ActiveWeaponSlot = PreviousActiveWeaponSlot;
                PreviousActiveWeaponSlot = newActiveWeaponSlot;
            }
            else
            {
                PreviousActiveWeaponSlot = ActiveWeaponSlot;
                ActiveWeaponSlot = newActiveWeaponSlot;
            }

            _weaponObjects[ActiveWeaponSlot]?.SetActive(true);
            _weaponObjects[PreviousActiveWeaponSlot]?.SetActive(false);
            EquipActiveWeapon();
        }

        /// <summary>
        /// Equips and Instantiates the Weapon, if there is one in the Weapon Slot
        /// </summary>
        /// <param name="weapon"></param>
        private void EquipActiveWeapon()
        {
            if (ActiveWeaponSlot == ActiveWeaponSlot.Unarmed)
            {
                var previousWeapon = _weaponObjects[PreviousActiveWeaponSlot].GetComponent<Weapon>();
                if (previousWeapon.BaseStats.AmmoType == AmmoType.Magazine)
                {
                    previousWeapon.CurrentStats.Spread.SetZero();
                }
                previousWeapon.GetComponent<Animator>().SetBool("Fire", false);
                HasWeapon = false;
            }
            else
            {
                if (CurrentWeapon.BaseStats.AmmoType == AmmoType.Magazine)
                {
                    CurrentWeapon.CurrentStats.Spread.SetZero();
                    if (_useSpreadSystem)
                        MessageHub.SendMessage(MessageType.SpreadChanged, CurrentWeapon.CurrentStats.Spread.Value);
                }
                CurrentWeapon.GetComponent<Animator>().SetBool("Fire", false);
                HasWeapon = true;
            }

            _weaponObjects[PreviousActiveWeaponSlot]?.SetActive(false);
            _weaponObjects[ActiveWeaponSlot]?.gameObject.SetActive(true);

            if (HasWeapon)
            {
                MessageHub.SendMessage(MessageType.WeaponChanged, CurrentWeapon.BaseStats.WeaponIcon, CurrentWeapon.BaseStats.AmmoType == AmmoType.Overheat);
                if (CurrentWeapon.BaseStats.AmmoType == AmmoType.Magazine)
                {
                    MessageHub.SendMessage(MessageType.AmmoChanged, CurrentWeapon.CurrentStats.Magazine.Value);
                }
                else
                {
                    MessageHub.SendMessage(MessageType.OverheatChanged, CurrentWeapon.CurrentStats.OverHeat.Value);
                }
            }
            else
            {
                MessageHub.SendMessage(MessageType.WeaponUnequipped);
                _photonIKView.DisableRightHand();
                _photonIKView.DisableLeftHand();
                _photonIKView.DisableNeck();
                _photonIKView.DisableUpperChest();
                _photonIKView.DisableChest();
            }
        }

        /// <summary>
        /// Handels the Shooting
        /// </summary>
        private void Shoot()
        {
            CurrentWeapon.IsShooting = true;
            MessageHub.SendMessage(MessageType.PlayWeaponSound, CurrentWeapon.IsShooting);
            //TODO: This makes the weapon (mostly ak47) on first frame of animation, don't know why
            _weaponObjects[ActiveWeaponSlot].GetComponent<Animator>().SetBool("Fire", true);

            if (CurrentWeapon.BaseStats.AmmoType == AmmoType.Magazine)
            {
                CurrentWeapon.Shoot();
            }

            try
            {
                if (CurrentWeapon.BaseStats.AmmoType == AmmoType.Magazine)
                {
                    StopCoroutine(_reloadCoroutine);
                    CurrentWeapon.CurrentStats.Magazine.IsReloading = false;
                }
            }
            catch (Exception) { }

            //TODO: another damage type calc and raycast for flamethrower and similar weapons?

            Vector3 shootDirection = _networkPlayer.FpsCamera.transform.forward;
            if (_useSpreadSystem)
            {
                var spreaValue = CurrentWeapon.CurrentStats.Spread.Value;
                shootDirection.x += Random.Range(-spreaValue, spreaValue);
                shootDirection.y += Random.Range(-spreaValue, spreaValue);
            }
            MessageHub.SendMessage(MessageType.WeaponShot, shootDirection);

            if (Physics.Raycast(_networkPlayer.FpsCamera.transform.position, shootDirection, out RaycastHit hit, CurrentWeapon.CurrentStats.Range.Value, _shootingLayerMask.value))
            {
                MessageHub.SendMessage(MessageType.WeaponHit, hit);

                int finalDamage = (int)(CurrentWeapon.CurrentStats.Damage.Value * _networkPlayer.CurrentStats.DamageModifier.PercentualValue
                    + _networkPlayer.CurrentStats.DamageModifier.FlatValue);

                if (_networkPlayer.RemotePlayerMask.value == (_damageableLayerMask.value & 1 << hit.transform.gameObject.layer))
                {
                    //We hit a player
                    var pView = hit.transform.GetComponent<PhotonView>();
                    var player = PhotonNetwork.LocalPlayer.Get(pView.OwnerActorNr);

                    pView.RPC("TakeDamage", player, finalDamage, photonView.ViewID);
                }
                else if (_damageableLayerMask.value == (_damageableLayerMask.value | 1 << hit.transform.gameObject.layer))
                {
                    var pView = hit.transform.GetComponent<PhotonView>();
                    pView.RPC("TakeDamage", RpcTarget.MasterClient, finalDamage, photonView.ViewID);
                }
            }
        }

        private IEnumerator Reload()
        {
            //TODO: Play ReloadAnimation and wait for AnimationEnd Event or solve only with the event and stop the animation on weapon change or when shooting
            yield return new WaitForSeconds(CurrentWeapon.CurrentStats.Magazine.ReloadTime);
            CurrentWeapon.Reload();
            CurrentWeapon.CurrentStats.Magazine.IsReloading = false;
            MessageHub.SendMessage(MessageType.AmmoChanged, CurrentWeapon.CurrentStats.Magazine.Value);
        }

        private IEnumerator CooldownPenaltyRoutine(Weapon weapon)
        {
            yield return new WaitForSeconds(weapon.BaseStats.OverHeat.CooldownInSeconds);
            weapon.CurrentStats.OverHeat.IsCoolingDown = false;
        }

        [PunRPC]
        internal void SwitchWeapon(int activeWeaponSlot, string weaponPrefabName)
        {
            if (photonView.IsMine)
            {
                _weaponObjects[(ActiveWeaponSlot)activeWeaponSlot].SetActive(false);
                foreach (Transform child in _networkPlayer.WeaponHolder.transform)
                {
                    if (child.name == weaponPrefabName)
                        _weaponObjects[(ActiveWeaponSlot)activeWeaponSlot] = child.gameObject;
                }
                _weaponObjects[(ActiveWeaponSlot)activeWeaponSlot].GetComponent<Weapon>().Init();

                int layer = (int)Math.Log(_weaponLayerMask.value, 2);
                _weaponObjects[(ActiveWeaponSlot)activeWeaponSlot].layer = layer;
                Transform[] children = _weaponObjects[(ActiveWeaponSlot)activeWeaponSlot].GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    child.gameObject.layer = layer;
                }

                if (CurrentWeapon.BaseStats.AmmoType == AmmoType.Magazine)
                {
                    CurrentWeapon.CurrentStats.Spread.SetZero();
                    MessageHub.SendMessage(MessageType.AmmoChanged, CurrentWeapon.CurrentStats.Magazine.Value);
                    if (_useSpreadSystem)
                        MessageHub.SendMessage(MessageType.SpreadChanged, CurrentWeapon.CurrentStats.Spread.Value);
                }
                else
                {
                    MessageHub.SendMessage(MessageType.OverheatChanged, CurrentWeapon.CurrentStats.OverHeat.Value);
                }

                MessageHub.SendMessage(MessageType.WeaponChanged, CurrentWeapon.BaseStats.WeaponIcon, CurrentWeapon.BaseStats.AmmoType == AmmoType.Overheat);
                CurrentWeapon.GetComponent<Animator>().SetBool("Fire", false);
                HasWeapon = true;
            }
        }
        #endregion
    }
}