using Photon.Pun;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects;
using WatStudios.DeepestDungeon.Core.EnemyLogic;
using WatStudios.DeepestDungeon.Core.Utiliy.Areas;

namespace WatStudios.DeepestDungeon.Core.WeaponLogic
{
    public class Grenade : MonoBehaviourPun
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private float _timeToExplode;
        [SerializeField] private GameObject _explosionEffect;
        [SerializeField] private SphereArea _areaOfEffect;
        [SerializeField] private LayerMask _afflictedObjects;
        [SerializeField] private LayerMask _bounceLayer;
        [SerializeField] private StatusEffect _appliedEffect;
        [SerializeField] private AnimationCurve _damageFallOffCurve;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private float _countdown;
        private bool _hasExploded;
        private bool _active;
        private int _flatDmgIncrease;
        private float _percDmgIncrease;
        #endregion

        #region Properties
        public LayerMask BounceLayer => _bounceLayer;
        #endregion

        #region MonoBehaviour Callbacks
        private void Update()
        {
            if (!photonView.IsMine)
                return;
            if (_active)
            {
                _countdown -= Time.deltaTime;
                if (_countdown <= 0 && !_hasExploded)
                    Explode();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start the grenade ticking countdown
        /// </summary>
        public void StartTicking()
        {
            _active = true;
            _countdown = _timeToExplode;
        }

        /// <summary>
        /// The damage bonus value of the player
        /// </summary>
        /// <param name="flatDmgIncrease">Flat Damage Increase</param>
        /// <param name="percDamageIncrease">Percentual Damage Increase</param>
        public void AddBonusDamage(int flatDmgIncrease, float percDamageIncrease)
        {
            _flatDmgIncrease = flatDmgIncrease;
            _percDmgIncrease = percDamageIncrease;
        }
        #endregion

        #region Private Methods
        private void Explode()
        {
            _hasExploded = true;
            //Show Effect
            PhotonNetwork.Instantiate("Particles/Abilities/" + _explosionEffect.name, transform.position, transform.rotation);
            //Damage
            foreach (var collider in _areaOfEffect.GetCollidingObjects(transform.position, transform.forward, _afflictedObjects))
            {
                var entity = collider.GetComponent<IStatusEntity>();
                if (_appliedEffect != null)
                {
                    if (!DatabaseManager.TryGetId(_appliedEffect, out int statusEffectId))
                        Debug.LogError("No such element in the database!");
                    entity.StatusEffectHandler.photonView.RPC("AddStatusEffect", RpcTarget.All, statusEffectId, photonView.ViewID);
                }
                float finalDamage = _damageFallOffCurve.Evaluate((collider.transform.position - transform.position).magnitude);
                finalDamage *= _percDmgIncrease;
                finalDamage += _flatDmgIncrease;
                entity.PhotonView.RPC("TakeDamage", RpcTarget.MasterClient, (int)finalDamage, photonView.ViewID);
            }
            PhotonNetwork.Destroy(gameObject);
        }
        #endregion
    }
}