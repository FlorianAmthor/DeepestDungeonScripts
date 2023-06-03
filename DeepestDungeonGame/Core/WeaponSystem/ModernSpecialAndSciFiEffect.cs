using Photon.Pun;
using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Utility.CustomPhoton;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.WeaponLogic
{
    public class ModernSpecialAndSciFiEffect : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private bool _useSciFi;
        [SerializeField] private bool _laserBeam;
        [SerializeField] private ParticleSystem[] _firingEffects;
        [SerializeField] private GameObject[] _impactEffects;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private Weapon CurrentWeapon { get => NetworkPlayer.LocalPlayerInstance.WeaponSystem.CurrentWeapon; }
        #endregion

        #region Public Fields
        [HideInInspector]
        public bool Shooting;
        #endregion

        #region MonoBehaviour Callbacks
        private void OnEnable()
        {
            MessageHub.Subscribe(MessageType.WeaponShot, OnWeaponShot, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.WeaponHit, OnWeaponHit, ActionExecutionScope.Default);
        }

        private void OnDisable()
        {
            MessageHub.Unsubscribe(MessageType.WeaponShot, OnWeaponShot);
            MessageHub.Unsubscribe(MessageType.WeaponHit, OnWeaponHit);
        }

        private void OnWeaponShot(Message obj)
        {
            foreach (ParticleSystem fire in _firingEffects)
            {
                if (fire != null)
                {
                    if (CurrentWeapon.CurrentStats.Spread.Value >= CurrentWeapon.CurrentStats.Spread.Factor)
                    {
                        Vector3 shootDirection = (Vector3)obj.Data[0];
                        if (_laserBeam)
                        { }//fire.transform.localRotation = Quaternion.LookRotation(shootDirection);
                        else
                            fire.transform.localRotation = Quaternion.Euler(shootDirection.x, shootDirection.y, shootDirection.z);
                    }
                    else
                    {
                        fire.transform.rotation.SetLookRotation(NetworkPlayer.LocalPlayerInstance.FpsCamera.transform.forward);
                    }

                    fire.Play();

                    fire.GetComponent<PhotonParticleView>().PlayParticleSystem(fire.transform.position, fire.transform.rotation);
                }
            }
        }

        #endregion

        #region Private Methods
        private void OnWeaponHit(Message obj)
        {
            var hit = (RaycastHit)obj.Data[0];
            foreach (GameObject impact in _impactEffects)
            {
                if (impact != null)
                    SpawnDecal(hit, impact);
            }
            Shooting = true;
        }

        private void SpawnDecal(RaycastHit hit, GameObject prefab)
        {
            if (_useSciFi)
            {
                GameObject spawnedDecal = PhotonNetwork.Instantiate("Particles/Sci-FiImpact/" + prefab.name, hit.point, Quaternion.LookRotation(hit.normal));
                spawnedDecal.transform.SetParent(hit.collider.transform);

                spawnedDecal.GetComponent<PhotonParticleView>().PlayParticleSystem(spawnedDecal.transform.position, spawnedDecal.transform.rotation);
                PhotonParticleView[] systems = spawnedDecal.GetComponentsInChildren<PhotonParticleView>();
                foreach (PhotonParticleView sys in systems)
                    sys.PlayParticleSystem(sys.transform.position, sys.transform.rotation);
            }
            else
            {
                GameObject spawnedDecal = PhotonNetwork.Instantiate("Particles/SpecialImpact/" + prefab.name, hit.point, Quaternion.LookRotation(hit.normal));
                spawnedDecal.transform.SetParent(hit.collider.transform);

                spawnedDecal.GetComponent<PhotonParticleView>().PlayParticleSystem(spawnedDecal.transform.position, spawnedDecal.transform.rotation);
                PhotonParticleView[] systems = spawnedDecal.GetComponentsInChildren<PhotonParticleView>();
                foreach (PhotonParticleView sys in systems)
                    sys.PlayParticleSystem(sys.transform.position, sys.transform.rotation);
            }

        }
        #endregion
    }
}