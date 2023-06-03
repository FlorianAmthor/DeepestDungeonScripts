using UnityEngine;

namespace WatStudios.DeepestDungeon.Audio
{
    /// <summary>
    /// This class updates the Sphere Collider which is set as a Sound Emitter,
    /// which is attached to the Player.
    /// </summary>
    public class SoundEmitterPlayer : MonoBehaviour
    {
        #region Private Fields
        private SphereCollider _collider = null;
        private float _srcRadius = 0.0f;
        private float _tgtRadius = 0.0f;
        private float _interpolator = 0.0f;
        private float _interpolatorSpeed = 0.0f;
        private float _currentRadius = 0.0f;
        #endregion

        #region Exposed Private Fields
        [SerializeField] private float _decayRate = 1.0f;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            // Cache Collider Reference
            _collider = GetComponent<SphereCollider>();
            if (!_collider) return; 

            // Set Radius Values
            _currentRadius = _srcRadius = _tgtRadius = _collider.radius;

            // Setup Interpolator
            _interpolator = 0.0f;
            if (_decayRate > 0.02f)
             _interpolatorSpeed = 1.0f * _decayRate; 
            else
             _interpolatorSpeed = 0.0f; 
        }

        void FixedUpdate()
        {
            if (!_collider) return; 
            _collider.radius = _currentRadius;
            if (_collider.radius < Mathf.Epsilon)
                _collider.enabled = false; 
            else 
                _collider.enabled = true; 
        }

        void Update()
        {
            _interpolator = Mathf.Clamp01(_interpolator + Time.deltaTime * _interpolatorSpeed);
            _currentRadius = Mathf.Lerp(_srcRadius, _tgtRadius, _interpolator);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the radius of the Sphere Collider
        /// </summary>
        /// <param name="newRadius"></param>
        /// <param name="instantResize"></param>
        public void SetRadius(float newRadius, bool instantResize = false)
        {
            if (!_collider || newRadius == _tgtRadius) return; 

            _srcRadius = _currentRadius = (instantResize || newRadius > _currentRadius) ? newRadius : _currentRadius;
            _tgtRadius = newRadius;
            _interpolator = 0.0f;
        }

        /// <summary>
        /// Display the explosion radius when selected
        /// </summary>
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, _currentRadius);
        }
        #endregion

    }
}
