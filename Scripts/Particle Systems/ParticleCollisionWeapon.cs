using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.WeaponLogic;
using WatStudios.DeepestDungeon.Utility.CustomPhoton;

namespace WatStudios.DeepestDungeon.Particles
{
    public class ParticleCollisionWeapon : MonoBehaviourPunCallbacks
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private bool continuesEffect;
        [SerializeField] private bool _useSciFi;
        [SerializeField, Tooltip("If not selected it will use the Special Impact Effects.")]
        private GameObject[] _impactEffects;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private List<ParticleCollisionEvent> _collisionEvents;
        private ParticleSystem _particle;
        private bool _changedLayer;
        private ModernSpecialAndSciFiEffect _effect;
        private PhotonParticleView[] _particleView;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            _particle = GetComponent<ParticleSystem>();
            _collisionEvents = new List<ParticleCollisionEvent>();
            _effect = GetComponentInParent<ModernSpecialAndSciFiEffect>();
            _particleView = GetComponentsInChildren<PhotonParticleView>();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (!_changedLayer)
                {
                    if (gameObject.layer == LayerMask.NameToLayer("Weapon"))
                    {
                        var col = _particle.collision;
                        int mask = ~((1 << LayerMask.NameToLayer("LocalPlayer")) | (1 << LayerMask.NameToLayer("Weapon")) | (1 << LayerMask.NameToLayer("RemotePlayer")));
                        col.collidesWith = mask;
                        _changedLayer = true;
                    }
                    else if (gameObject.layer == LayerMask.NameToLayer("LocalPlayer"))
                    {
                        var col = _particle.collision;
                        int mask = ~((1 << LayerMask.NameToLayer("Weapon")) | (1 << LayerMask.NameToLayer("LocalPlayer")) | (1 << LayerMask.NameToLayer("RemotePlayer")));
                        col.collidesWith = mask;
                        _changedLayer = true;
                    }
                }
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            if (_effect.Shooting)
            {
                int numCollisionEvents = _particle.GetCollisionEvents(other, _collisionEvents);
                int i = 0;

                while (i < numCollisionEvents)
                {
                    var colEvent = _collisionEvents[i];
                    if (_useSciFi)
                    {
                        foreach (GameObject impact in _impactEffects)
                        {
                            if (impact != null)
                            {
                                GameObject spawnedDecal = PhotonNetwork.Instantiate("Particles/Sci-FiImpact/" + impact.name, colEvent.intersection, Quaternion.LookRotation(colEvent.normal));
                                spawnedDecal.transform.SetParent(colEvent.colliderComponent.transform);

                                spawnedDecal.GetComponent<PhotonParticleView>().PlayParticleSystem(spawnedDecal.transform.position, spawnedDecal.transform.rotation);
                                PhotonParticleView[] systems = spawnedDecal.GetComponentsInChildren<PhotonParticleView>();
                                foreach (PhotonParticleView sys in systems)
                                    sys.PlayParticleSystem(sys.transform.position, sys.transform.rotation);

                                _collisionEvents = new List<ParticleCollisionEvent>();
                                _effect.Shooting = false;
                                if (!continuesEffect)
                                    _particle.Stop();

                                return;
                            }
                        }
                    }
                    else
                    {
                        foreach (GameObject impact in _impactEffects)
                        {
                            if (impact != null)
                            {
                                GameObject spawnedDecal = PhotonNetwork.Instantiate("Particles/SpecialImpact/" + impact.name, colEvent.intersection, Quaternion.LookRotation(colEvent.normal));
                                spawnedDecal.transform.SetParent(colEvent.colliderComponent.transform);

                                spawnedDecal.GetComponent<PhotonParticleView>().PlayParticleSystem(spawnedDecal.transform.position, spawnedDecal.transform.rotation);
                                PhotonParticleView[] systems = spawnedDecal.GetComponentsInChildren<PhotonParticleView>();
                                foreach (PhotonParticleView sys in systems)
                                    sys.PlayParticleSystem(sys.transform.position, sys.transform.rotation);

                                _collisionEvents = new List<ParticleCollisionEvent>();
                                _effect.Shooting = false;
                                if (!continuesEffect)
                                    _particle.Stop();

                                return;
                            }
                        }
                    }
                    i++;
                }
            }
        }
        #endregion
    }
}