using UnityEngine;

namespace WatStudios.DeepestDungeon.Audio
{
    /// <summary>
    /// This class updates the Sphere Collider which is set as a Sound Emitter,
    /// which is attached to the Player.
    /// </summary>

    //[CreateAssetMenu(menuName = "ScriptableObjects/Audio/Sound Emitter", fileName = "New Sound Emitter")]
    public class GeneralSoundEmitter : MonoBehaviour
    {
        #region Private Fields
        private SphereCollider _collider;

        private float _srcRadius;
        private float _tgtRadius;
        private float _interpolator;
        private float _interpolatorSpeed;
        private float _currentRadius;


        #endregion

        #region Public Fields
        public float DecayRate;
        public float SoundRadius;
        public bool KeepRunning;
        public float DecreaseRadiusMultiplier;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _collider.radius = SoundRadius;
        }

        void Start()
        {
            if (!_collider) return;

            Init();
        }

        void FixedUpdate()
        {
            if (!_collider) return;

            _collider.radius = _currentRadius;
            if (_collider.radius <= 0.01f)
                Destroy(gameObject);
            else
                _collider.enabled = true;
        }

        void Update()
        {
            if (!_collider) return;
            if (_currentRadius <= (SoundRadius * DecreaseRadiusMultiplier) + 0.01f) return;

            _interpolator = Mathf.Clamp01(_interpolator + Time.deltaTime * _interpolatorSpeed);
            _currentRadius = Mathf.Lerp(_srcRadius, _tgtRadius, _interpolator);

            CalculateRadius();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the Functions
        /// </summary>
        private void Init()
        {
            // Set Radius Values
            _currentRadius = _srcRadius = _tgtRadius = _collider.radius;

            // Setup Interpolator
            _interpolator = 0.0f;
            if (DecayRate > 0.02f)
                _interpolatorSpeed = 1.0f * DecayRate;
            else
                _interpolatorSpeed = 0.0f;
            if (DecreaseRadiusMultiplier != 0)
                KeepRunning = true;
        }

        /// <summary>
        /// Calculate the SoundEmitter radius
        /// </summary>
        private void CalculateRadius()
        {
            float newRadius = 0;
            if (KeepRunning == true)
                newRadius = Mathf.Max(0, (SoundRadius * DecreaseRadiusMultiplier));

            SetRadius(newRadius);
        }

        /// <summary>
        /// Sets the radius of the Sphere Collider
        /// </summary>
        /// <param name="newRadius"></param>
        /// <param name="instantResize"></param>
        private void SetRadius(float newRadius, bool instantResize = false)
        {
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
            Gizmos.DrawWireSphere(gameObject.transform.position, _currentRadius);
        }
        #endregion

    }
}
