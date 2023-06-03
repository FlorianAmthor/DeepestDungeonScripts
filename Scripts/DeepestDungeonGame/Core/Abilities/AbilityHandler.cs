using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities
{
    public class AbilityHandler : MonoBehaviourPun
    {
        #region Exposed Private Fields
#pragma warning disable 649
        private List<Ability> _abilities;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private bool[] _abilityPreviewActive;
        private int _abilityUsed;
        private NetworkPlayer _nPlayer;
        #endregion

        #region Properties
        public bool PreviewActive { get; private set; }
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (!photonView.IsMine)
                return;
            CheckForAbilityInputs();
            foreach (var ability in _abilities)
            {
                ability.Tick(_nPlayer);
            }
        }
        #endregion

        #region Private Methods
        private void CheckForAbilityInputs()
        {
            if (!NetworkPlayer.AllowPlayerInput || !photonView.IsMine)
                return;

            if (_abilityUsed != -1)
            {
                PreviewActive = false;
                _abilities[_abilityUsed].Preview(false, false, _nPlayer);
                _abilityPreviewActive[_abilityUsed] = false;
                _abilityUsed = -1;
            }

            for (int i = 0; i < _abilities.Count; i++)
            {
                if (_abilities[i].IsUsable)
                {
                    switch (_abilities[i].AbilityActivation)
                    {
                        case AbilityActivation.Aimable:
                            if (_abilities[i].UseQuickCast)
                            {
                                if (!_abilityPreviewActive[i])
                                    _abilityPreviewActive[i] = Input.GetButtonDown("Ability" + (i + 1));
                            }
                            else
                            {
                                if (!_abilityPreviewActive[i])
                                    _abilityPreviewActive[i] = Input.GetButtonDown("Ability" + (i + 1));
                            }

                            if (_abilityPreviewActive[i])
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    _abilityUsed = i;
                                    PreviewActive = true;
                                    _abilities[i].Use(_nPlayer, Input.GetButton("SelfCast"));
                                    return;
                                }
                                else if (Input.GetMouseButtonDown(1))
                                {
                                    _abilities[i].Preview(false, true);
                                    _abilityPreviewActive[i] = false;
                                    return;
                                }
                                else
                                {
                                    _abilities[i].Preview(true, false, _nPlayer);
                                    _abilityPreviewActive[i] = true;
                                }
                            }

                            if (_abilities[i].UseQuickCast && _abilityPreviewActive[i] && Input.GetButtonUp("Ability" + (i + 1)))
                            {
                                _abilityUsed = i;
                                PreviewActive = true;
                                _abilities[i].Use(_nPlayer, Input.GetButton("SelfCast"));
                                return;
                            }
                            break;
                        case AbilityActivation.Instant:
                        case AbilityActivation.Toggle:
                            if (Input.GetButton("Ability" + (i + 1)))
                            {
                                _abilityUsed = i;
                                PreviewActive = true;
                                _abilities[i].Use(_nPlayer, Input.GetButton("SelfCast"));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public void Init()
        {
            if (!photonView.IsMine)
                return;

            _nPlayer = GetComponent<NetworkPlayer>();
            _abilities = _nPlayer.BaseStats.Abilities;

            foreach (var ability in _abilities)
            {
                ability.Cooldown.SetZero();
                MessageHub.SendMessage(MessageType.AbilityInfo, ability.Name, ability.AbilityImage, ability.Cooldown.BaseValue, _abilities.IndexOf(ability));
            }

            _abilityPreviewActive = new bool[_abilities.Count];
            for (int i = 0; i < _abilityPreviewActive.Length; i++)
            {
                _abilityPreviewActive[i] = false;
            }
        }
        #endregion
    }
}