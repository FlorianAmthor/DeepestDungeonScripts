using Photon.Pun;
using UnityEngine;
using WatStudios.DeepestDungeon.Messaging;
using WatStudios.DeepestDungeon.Utility.CustomPhoton;
using Random = UnityEngine.Random;

namespace WatStudios.DeepestDungeon.Core.WeaponLogic
{
    public class ModernGunEffect : MonoBehaviour
    {

        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private ParticleSystem[] _firingEffects;
        [SerializeField] private GameObject _metalHitEffect;
        [SerializeField] private GameObject _sandHitEffect;
        [SerializeField] private GameObject _stoneHitEffect;
        [SerializeField] private GameObject _waterLeakEffect;
        [SerializeField] private GameObject _waterLeakExtinguishEffect;
        [SerializeField] private GameObject[] _fleshHitEffects;
        [SerializeField] private GameObject _woodHitEffect;
        [SerializeField] private GameObject _defaultEffect;
#pragma warning restore 649
        #endregion

        #region MonoBehaviour Callbacks
        private void OnEnable()
        {
            MessageHub.Subscribe(MessageType.WeaponHit, OnWeaponHit, ActionExecutionScope.Default);
            MessageHub.Subscribe(MessageType.WeaponShot, OnWeaponShot, ActionExecutionScope.Default);
        }

        private void OnDisable()
        {
            MessageHub.Unsubscribe(MessageType.WeaponHit, OnWeaponHit);
            MessageHub.Unsubscribe(MessageType.WeaponShot, OnWeaponShot);
        }

        private void OnWeaponHit(Message obj)
        {
            HandleHit((RaycastHit)obj.Data[0]);
        }
        #endregion

        #region Private Methods
        private void OnWeaponShot(Message obj)
        {
            foreach (ParticleSystem fire in _firingEffects)
            {
                if (fire != null)
                {
                    //Local Play Particle because Custom PhotonParticeView doesn't play the particle locally
                    fire.Play();
                    fire.GetComponent<PhotonParticleView>().PlayParticleSystem(fire.transform.position, fire.transform.rotation);
                }
            }
        }

        private void HandleHit(RaycastHit hit)
        {
            if (hit.collider.sharedMaterial != null)
            {
                string materialName = hit.collider.sharedMaterial.name;

                switch (materialName)
                {

                    case "Metal":
                        SpawnDecal(hit, _metalHitEffect);
                        break;
                    case "Sand":
                        SpawnDecal(hit, _sandHitEffect);
                        break;
                    case "Stone":
                        SpawnDecal(hit, _stoneHitEffect);
                        break;
                    case "WaterFilled":
                        SpawnDecal(hit, _waterLeakEffect);
                        SpawnDecal(hit, _metalHitEffect);
                        break;
                    case "Wood":
                        SpawnDecal(hit, _woodHitEffect);
                        break;
                    case "Meat":
                        SpawnDecal(hit, _fleshHitEffects[Random.Range(0, _fleshHitEffects.Length)]);
                        break;
                    case "Player":
                        SpawnDecal(hit, _fleshHitEffects[Random.Range(0, _fleshHitEffects.Length)]);
                        break;
                    case "WaterFilledExtinguish":
                        SpawnDecal(hit, _waterLeakExtinguishEffect);
                        SpawnDecal(hit, _metalHitEffect);
                        break;
                    default:
                        SpawnDecal(hit, _defaultEffect);
                        break;
                }
            }
            else
            {
                SpawnDecal(hit, _defaultEffect);
            }
        }

        private void SpawnDecal(RaycastHit hit, GameObject prefab)
        {
            GameObject spawnedDecal = PhotonNetwork.Instantiate("Particles/BulletImpact/" + prefab.name, hit.point, Quaternion.LookRotation(hit.normal));
            spawnedDecal.transform.SetParent(hit.collider.transform);

            foreach (ParticleSystem particle in spawnedDecal.GetComponentsInChildren<ParticleSystem>())
                particle.GetComponent<PhotonParticleView>().PlayParticleSystem(particle.transform.position, particle.transform.rotation);
        }
        #endregion 
    }
}