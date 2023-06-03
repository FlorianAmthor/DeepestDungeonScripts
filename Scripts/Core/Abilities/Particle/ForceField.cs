using UnityEngine;
using WatStudios.DeepestDungeon.Utility.CustomPhoton;

namespace WatStudios.DeepestDungeon.AbilitySystem.Particles
{
    public class ForceField : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private LayerMask enemyLayer;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private ParticleSystem particle;
        private PhotonParticleView particleView;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            particle = GetComponentInChildren<ParticleSystem>();
            particleView = GetComponentInChildren<PhotonParticleView>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (enemyLayer == (enemyLayer | 1 << collision.gameObject.layer))
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    particle.transform.position = contact.point;
                    particle.transform.LookAt(transform.position);
                    particle.Play();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (enemyLayer == (enemyLayer | 1 << other.gameObject.layer))
            {
                particle.transform.position = other.transform.position;
                particle.transform.LookAt(transform.position);
                particle.Play();

                particleView.PlayParticleSystem(particle.transform.position, particle.transform.rotation);
            }
        }
        #endregion
    }
}
