using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Networking;

namespace WatStudios.DeepestDungeon.UI.Gameplay
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields
        private GameObject _target;
        private Dictionary<Player, GameObject> _playerReadyCheckDict;
        private Dictionary<Player, GameObject> _playerGroupHealthUIDict;
        private Dictionary<string, GameObject> _playerAbilityDict;
        private Dictionary<Type, GameObject> _buffDict;
        private string _interactionButtonString = "E";
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [Header("Player UI")]
        [Tooltip("UI Image to display Player's Health")]
        [SerializeField] private Image _playerHealthImage;
        [Tooltip("UI Text to display Player's health in percent")]
        [SerializeField] private TextMeshProUGUI _playerHealthText;
        [SerializeField] private GameObject _currentAmmoUI;
        [SerializeField] private TextMeshProUGUI _currentAmmoText;
        [SerializeField] private GameObject _overheatUI;
        [SerializeField] private TextMeshProUGUI _overheatText;
        [SerializeField] private Image _overheatBarImage;
        [SerializeField] private Image _currentWeaponIcon;
        [SerializeField] private SimpleDynamicCrosshair _crosshair;
        [SerializeField] private Compass _compass;
        [SerializeField] private GameObject _groupUIGameObject;
        [SerializeField] private GameObject _groupHealthUIPrefab;

        [Header("AbilityUI")]
        [SerializeField] private GameObject _abilityHolder;
        [SerializeField] private GameObject _abilityUIPrefab;

        [Header("PlayerReadyUI")]
        [SerializeField] private GameObject _playerReadyUI;
        [SerializeField] private GameObject _playerReadyImageContainer;
        [SerializeField] private GameObject _playerReadyUIPrefab;

        [Header("Interaction UI")]
        [SerializeField] private GameObject _interactUiContainer;
        [SerializeField] private Image _interactionImage;
        [SerializeField] private Image _interactionForbiddenImage;
        [SerializeField] private TextMeshProUGUI _interactionImageText;
        [SerializeField] private TextMeshProUGUI _interactionText;

        [Header("Buff UI")]
        [SerializeField] private GameObject _buffUiPrefab;
        [SerializeField] private Transform _buffUiHolder;

        [Header("Objectives")]
        [SerializeField] private GameObject _objectiveUi;
        [SerializeField] private TextMeshProUGUI _objectiveText;
#pragma warning restore 649
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            _playerReadyCheckDict = new Dictionary<Player, GameObject>();
            _playerGroupHealthUIDict = new Dictionary<Player, GameObject>();
            _playerAbilityDict = new Dictionary<string, GameObject>();
            _buffDict = new Dictionary<Type, GameObject>();

            _interactionImageText.text = _interactionButtonString;
            _interactionImageText.enabled = true;
            _interactionText.enabled = true;
            _interactUiContainer.SetActive(false);
            foreach (var player in NetworkManager.Instance.PlayerList)
            {
                if (!player.IsLocal)
                {
                    AddPlayerToGroupUI(player);
                }
                AddPlayerReadyUI(player);
            }

            if (_playerGroupHealthUIDict.Count == 0)
                _groupUIGameObject.GetComponent<Image>().enabled = false;
            _playerReadyUI.SetActive(false);
        }

        private void Update()
        {
            if (_target == null)
            {
                Destroy(gameObject);
                return;
            }
        }
        private void OnEnable()
        {
            MessageHub.Subscribe(MessageType.PlayerJoinedRoom, OnPlayerJoinRoom, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.PlayerLeftRoom, OnPlayerLeftRoom, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.PlayerHealthChanged, OnHealthChange, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.AmmoChanged, OnAmmoChange, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.SpreadChanged, OnSpreadChanged, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.WeaponChanged, OnWeaponChange, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.WeaponUnequipped, OnWeaponUnequiped, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.InteractionEnter, OnInterActionEnter, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.InteractionExit, OnInterActionExit, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.AbilityCooldown, OnAbilityCooldown, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.AbilityCooldownStart, OnAbilityCooldownStart, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.AbilityCooldownEnd, OnAbilityCooldownEnd, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.AbilityInfo, OnAbilityInfo, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.InteractionForbidden, OnInterActionForbidden, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.ReadyCheckFail, OnReadyCheckFail, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.ReadyCheckInit, OnReadyCheckInit, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.ReadyCheckResponse, OnReadyCheckResponse, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.AllPlayersReady, OnAllPlayersReady, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.OverheatChanged, OnOverheatChanged, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.OverheatCooldown, OnOverheatCooldown, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.GameplaySceneLoaded, OnGameplaySceneLoaded, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.BuffStart, OnBuffStart, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.BuffTimeLeft, OnBuffTimeLeft, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.BuffEnd, OnBuffEnd, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.BuffRefresh, OnBuffRefresh, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.BuffStackChange, OnBuffStackChange, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.KillObjectiveData, OnKillObjectiveData, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.LobbySceneLoaded, OnLobbySceneLoaded, ActionExecutionScope.UserInterface);
            MessageHub.Subscribe(MessageType.GamePlaySceneBuilt, OnGamePlaySceneBuilt, ActionExecutionScope.UserInterface);
            //MessageHub.Subscribe(MessageType.LobbySceneLoaded, OnLobbySceneLoaded, ActionExecutionScope.UserInterface);
        }

        private void OnDisable()
        {
            MessageHub.Unsubscribe(MessageType.PlayerJoinedRoom, OnPlayerJoinRoom);
            MessageHub.Unsubscribe(MessageType.PlayerLeftRoom, OnPlayerLeftRoom);
            MessageHub.Unsubscribe(MessageType.PlayerHealthChanged, OnHealthChange);
            MessageHub.Unsubscribe(MessageType.AmmoChanged, OnAmmoChange);
            MessageHub.Unsubscribe(MessageType.SpreadChanged, OnSpreadChanged);
            MessageHub.Unsubscribe(MessageType.WeaponChanged, OnWeaponChange);
            MessageHub.Unsubscribe(MessageType.WeaponUnequipped, OnWeaponUnequiped);
            MessageHub.Unsubscribe(MessageType.InteractionEnter, OnInterActionEnter);
            MessageHub.Unsubscribe(MessageType.InteractionExit, OnInterActionExit);
            MessageHub.Unsubscribe(MessageType.AbilityCooldown, OnAbilityCooldown);
            MessageHub.Unsubscribe(MessageType.AbilityCooldownStart, OnAbilityCooldownStart);
            MessageHub.Unsubscribe(MessageType.AbilityCooldownEnd, OnAbilityCooldownEnd);
            MessageHub.Unsubscribe(MessageType.AbilityInfo, OnAbilityInfo);
            MessageHub.Unsubscribe(MessageType.InteractionForbidden, OnInterActionForbidden);
            MessageHub.Unsubscribe(MessageType.ReadyCheckInit, OnReadyCheckInit);
            MessageHub.Unsubscribe(MessageType.ReadyCheckResponse, OnReadyCheckResponse);
            MessageHub.Unsubscribe(MessageType.ReadyCheckFail, OnReadyCheckFail);
            MessageHub.Unsubscribe(MessageType.AllPlayersReady, OnAllPlayersReady);
            MessageHub.Unsubscribe(MessageType.OverheatChanged, OnOverheatChanged);
            MessageHub.Unsubscribe(MessageType.OverheatCooldown, OnOverheatCooldown);
            MessageHub.Unsubscribe(MessageType.GameplaySceneLoaded, OnGameplaySceneLoaded);
            MessageHub.Unsubscribe(MessageType.BuffStart, OnBuffStart);
            MessageHub.Unsubscribe(MessageType.BuffTimeLeft, OnBuffTimeLeft);
            MessageHub.Unsubscribe(MessageType.BuffEnd, OnBuffEnd);
            MessageHub.Unsubscribe(MessageType.BuffRefresh, OnBuffRefresh);
            MessageHub.Unsubscribe(MessageType.BuffStackChange, OnBuffStackChange);
            MessageHub.Unsubscribe(MessageType.KillObjectiveData, OnKillObjectiveData);
            MessageHub.Unsubscribe(MessageType.LobbySceneLoaded, OnLobbySceneLoaded);
            MessageHub.Unsubscribe(MessageType.GamePlaySceneBuilt, OnGamePlaySceneBuilt);
            //MessageHub.Unsubscribe(MessageType.LobbySceneLoaded, OnLobbySceneLoaded);
        }
        #endregion

        #region Messaging Callbacks
        private void OnPlayerJoinRoom(Message obj)
        {
            Player player = obj.Data[0] as Player;
            _groupUIGameObject.GetComponent<Image>().enabled = true;
            AddPlayerToGroupUI(player);
            AddPlayerReadyUI(player);
        }

        private void OnPlayerLeftRoom(Message obj)
        {
            Player p = obj.Data[0] as Player;
            if (_playerGroupHealthUIDict.TryGetValue(p, out GameObject playerGroupHealthObj))
            {
                _playerGroupHealthUIDict.Remove(p);
                Destroy(playerGroupHealthObj);
            }
            else
                Debug.LogError("No such player in the local cache");

            if (_playerReadyCheckDict.TryGetValue(p, out GameObject playerReadyCheckObj))
            {
                _playerReadyCheckDict.Remove(p);
                Destroy(playerReadyCheckObj);
            }
            else
                Debug.LogError("No such player in the local cache");
            if (_playerGroupHealthUIDict.Count == 0)
                _groupUIGameObject.GetComponent<Image>().enabled = false;
        }

        private void OnSpreadChanged(Message obj)
        {
            _crosshair.ChangeSize((float)obj.Data[0]);
        }

        private void OnHealthChange(Message obj)
        {
            ModifiyPlayerHealthUI((float)obj.Data[0], (int)obj.Data[1]);
        }

        private void OnAmmoChange(Message obj)
        {
            int ammo = (int)obj.Data[0];
            _currentAmmoText.text = ammo.ToString();
        }

        private void OnOverheatChanged(Message obj)
        {
            float overheatAmount = (float)obj.Data[0];
            _overheatText.text = ((int)(overheatAmount * 100.0f)).ToString() + "%";
            _overheatBarImage.fillAmount = overheatAmount;
        }

        private void OnOverheatCooldown(Message obj)
        {
            if ((bool)obj.Data[0]) //StartCoroutine blinkin
                Debug.Log("Overheat start blinking");
            else //StartCoroutine blinkin
                Debug.Log("Overheat stop blinking");
        }

        private void OnWeaponChange(Message obj)
        {
            Sprite weaponIcon = obj.Data[0] as Sprite;
            bool isOverheatWeapon = (bool)obj.Data[1];
            _currentAmmoUI.SetActive(!isOverheatWeapon);
            SetChildrenActive(_currentAmmoUI, !isOverheatWeapon);
            SetChildrenActive(_overheatUI, !isOverheatWeapon);
            _overheatUI.SetActive(isOverheatWeapon);
            _currentWeaponIcon.enabled = true;
            _currentWeaponIcon.enabled = true;
            _currentWeaponIcon.sprite = weaponIcon;
            _crosshair.ResetToZero();
            _crosshair.SetVisibility(true);
        }

        private void OnWeaponUnequiped(Message obj)
        {
            SetChildrenActive(_currentAmmoUI, false);
            SetChildrenActive(_overheatUI, false);
            _currentWeaponIcon.enabled = false;
            _crosshair.SetVisibility(false);
        }

        private void OnInterActionEnter(Message obj)
        {
            _interactUiContainer.SetActive(true);
            _interactionForbiddenImage.enabled = false;
            _interactionImageText.enabled = true;
            _interactionText.text = obj.Data[0] as string;
        }

        private void OnInterActionExit(Message obj)
        {
            DisableInteractionUI();
        }

        private void OnInterActionForbidden(Message obj)
        {
            _interactUiContainer.SetActive(true);
            _interactionForbiddenImage.enabled = true;
            _interactionImageText.enabled = false;
            _interactionText.text = obj.Data[0] as string;
        }

        private void OnAbilityCooldownStart(Message obj)
        {
            string abilityName = (string)obj.Data[0];
            if (_playerAbilityDict.TryGetValue(abilityName, out GameObject abilityUIObj))
                abilityUIObj.GetComponent<AbilityUI>().SwitchCooldownState(true);
            else
                Debug.LogError($"{abilityName} is not yet registered!");
        }

        private void OnAbilityCooldownEnd(Message obj)
        {
            string abilityName = (string)obj.Data[0];
            if (_playerAbilityDict.TryGetValue(abilityName, out GameObject abilityUIObj))
                abilityUIObj.GetComponent<AbilityUI>().SwitchCooldownState(false);
            else
                Debug.LogError($"{abilityName} is not yet registered!");
        }

        private void OnAbilityCooldown(Message obj)
        {
            string abilityName = (string)obj.Data[0];
            float cooldownLeft = (float)obj.Data[1];
            if (_playerAbilityDict.TryGetValue(abilityName, out GameObject abilityUIObj))
                abilityUIObj.GetComponent<AbilityUI>().UpdateUI(cooldownLeft);
        }

        private void OnAbilityInfo(Message obj)
        {
            string abilityName = (string)obj.Data[0];
            Sprite abilityImage = (Sprite)obj.Data[1];
            float abilityCooldown = (float)obj.Data[2];
            int abilityIndex = (int)obj.Data[3];
            if (!_playerAbilityDict.ContainsKey(abilityName))
            {
                var abilityUiObj = Instantiate(_abilityUIPrefab, _abilityHolder.transform);
                abilityUiObj.GetComponent<AbilityUI>().Init(abilityImage, abilityCooldown, abilityIndex);
                _playerAbilityDict.Add(abilityName, abilityUiObj);

                if (_playerAbilityDict.Count == 3)
                {
                    var transforms = new Transform[3];
                    foreach (Transform t in _abilityHolder.transform)
                    {
                        var abilityUI = t.GetComponent<AbilityUI>();
                        transforms[abilityUI.AbilityIndex] = t;
                    }
                    _abilityHolder.transform.DetachChildren();
                    foreach (var t in transforms)
                    {
                        t.SetParent(_abilityHolder.transform, false);
                    }
                }
            }
        }

        private void OnReadyCheckInit(Message obj)
        {
            _playerReadyUI.SetActive(true);
        }

        private void OnReadyCheckResponse(Message obj)
        {
            bool isReady = (bool)obj.Data[1];
            Player player = obj.Data[0] as Player;
            if (_playerReadyCheckDict.TryGetValue(player, out GameObject playerReadyObj))
            {
                playerReadyObj.GetComponent<PlayerReadyUI>().SetReady(true);
            }
            if (!isReady)
                Debug.Log($"Ready check failed! {player.NickName} is not ready.");
        }

        private void OnReadyCheckFail(Message obj)
        {
            ResetReadyCheck();
        }

        private void OnAllPlayersReady(Message obj)
        {
            ResetReadyCheck();
        }
        private void OnGameplaySceneLoaded(Message obj)
        {
            DisableInteractionUI();
            //TODO: show objectives ui?
        }

        private void OnLobbySceneLoaded(Message obj)
        {
            _objectiveUi.SetActive(false);
        }

        private void OnGamePlaySceneBuilt(Message obj)
        {
            _objectiveUi.SetActive(true);
        }

        private void OnKillObjectiveData(Message obj)
        {
            int monsterSoFar = (int)obj.Data[0];
            int monsterMax = (int)obj.Data[1];

            _objectiveText.text = $"Monsters killed: {monsterSoFar}/{monsterMax}";
        }

        private void OnBuffStart(Message obj)
        {
            Type buffType = (Type)obj.Data[0];
            Sprite buffImage = obj.Data[1] as Sprite;
            bool isBuff = (bool)obj.Data[2];
            bool isStackable = (bool)obj.Data[3];
            float maxDuration = (float)obj.Data[4];

            BuffUI buffUI;

            if (!_buffDict.ContainsKey(buffType))
            {
                var buffUi = Instantiate(_buffUiPrefab, _buffUiHolder).GetComponent<BuffUI>();
                buffUi.Init(buffImage, isBuff, isStackable, maxDuration);
                _buffDict.Add(buffType, buffUi.gameObject);
                buffUI = _buffDict[buffType].GetComponent<BuffUI>();
            }
            else
            {
                _buffDict[buffType].GetComponent<BuffUI>().OnCooldownChange(maxDuration);
            }
        }

        private void OnBuffRefresh(Message obj)
        {
            Type buffType = (Type)obj.Data[0];
            int numOfStacks = (int)obj.Data[1];
            float newDuration = (float)obj.Data[2];
            if (_buffDict.ContainsKey(buffType))
                _buffDict[buffType].GetComponent<BuffUI>().OnBuffRefresh(numOfStacks);
        }

        private void OnBuffTimeLeft(Message obj)
        {
            Type buffType = (Type)obj.Data[0];
            float newDuration = (float)obj.Data[1];

            if (_buffDict.ContainsKey(buffType))
                _buffDict[buffType].GetComponent<BuffUI>().OnCooldownChange(newDuration);
        }

        private void OnBuffEnd(Message obj)
        {
            Type buffType = (Type)obj.Data[0];
            var buffObj = _buffDict[buffType];
            _buffDict.Remove(buffType);
            Destroy(buffObj);
        }

        private void OnBuffStackChange(Message obj)
        {
            Type buffType = (Type)obj.Data[0];
            int numOfStacks = (int)obj.Data[1];

            _buffDict[buffType].GetComponent<BuffUI>().OnStackNumberChange(numOfStacks);
        }

        #endregion

        #region Private Methods
        private void DisableInteractionUI()
        {
            _interactUiContainer.SetActive(false);
        }

        private void AddPlayerToGroupUI(Player player)
        {
            if (_playerGroupHealthUIDict.ContainsKey(player))
            {
                _playerGroupHealthUIDict.Remove(player);
            }
            GameObject gObj = Instantiate(_groupHealthUIPrefab, _groupUIGameObject.transform);
            _playerGroupHealthUIDict.Add(player, gObj);
            PlayerGroupUI ui = gObj.GetComponent<PlayerGroupUI>();
            ui.SetPlayerName(player.NickName);
            ui.SetPlayerHealthUI(1f);
        }

        private void AddPlayerReadyUI(Player player)
        {
            if (!_playerReadyCheckDict.ContainsKey(player))
            {
                GameObject gObj = Instantiate(_playerReadyUIPrefab, _playerReadyImageContainer.transform);
                _playerReadyCheckDict.Add(player, gObj);
                gObj.GetComponent<PlayerReadyUI>().SetReady(false);
            }
        }

        private void SetChildrenActive(GameObject obj, bool visible)
        {
            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// Resets the ReadyCheckUI
        /// </summary>
        private void ResetReadyCheck()
        {
            foreach (var value in _playerReadyCheckDict.Values)
            {
                value.GetComponent<PlayerReadyUI>().SetReady(false);
            }
            _playerReadyUI.SetActive(false);
        }

        /// <summary>
        /// Updates the health image and text according to <paramref name="healthPercent"/>
        /// </summary>
        /// <param name="healthPercent">Health in percent as float between 0.0f and 1.0f</param>
        private void ModifiyPlayerHealthUI(float healthPercent, int actorNumber)
        {
            var uiHealthPercent = healthPercent;

            if (uiHealthPercent > 0.0000f && uiHealthPercent <= 0.01f)
                uiHealthPercent = 0.01f;

            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            {
                _playerHealthImage.fillAmount = uiHealthPercent;
                uiHealthPercent *= 100.0f;
                string healthPercentString = ((int)uiHealthPercent).ToString();
                _playerHealthText.text = $"{healthPercentString}%";
            }
            else
            {
                Player playerToUpdate = PhotonNetwork.LocalPlayer.Get(actorNumber);
                if (_playerGroupHealthUIDict.TryGetValue(playerToUpdate, out GameObject playerGroupUI))
                {
                    playerGroupUI.GetComponent<PlayerGroupUI>().SetPlayerHealthUI(uiHealthPercent);
                }
                else
                    Debug.LogError("No such player in the local cache");
            }
        }
        #endregion

        #region Public Methods

        public void SetTarget(GameObject target)
        {
            if (target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Networkplayer target for PlayerUI.SetTarget.", this);
                return;
            }
            // Cache references for efficiency
            _target = target;
            _compass.Player = target.transform;
        }

        public void SetInteractionText(string text)
        {
            if (_interactionText)
            {
                if (text == null || text == string.Empty)
                {
                    _interactionText.text = string.Empty;
                    _interactionText.gameObject.SetActive(false);
                }
                else
                {
                    _interactionText.text = text;
                    _interactionText.gameObject.SetActive(true);
                }
            }
        }
        #endregion
    }
}
