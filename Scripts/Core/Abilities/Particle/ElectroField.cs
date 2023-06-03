using UnityEngine;
using WatStudios.DeepestDungeon.Utility.CustomPhoton;

namespace WatStudios.DeepestDungeon.AbilitySystem.Particles
{
    public class ElectroField : MonoBehaviour
    {
        private ParticleSystem _particles;
        private PhotonParticleView _particleView;

        //[SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float damagePerLightning;

        // Start is called before the first frame update
        private void Start()
        {
            _particles = GetComponent<ParticleSystem>();

            _particleView = GetComponent<PhotonParticleView>();
            _particleView.PlayParticleSystem(transform.position, transform.rotation);
        }
    }
}
