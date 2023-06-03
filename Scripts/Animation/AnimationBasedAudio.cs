using UnityEngine;
using WatStudios.DeepestDungeon.Audio;

namespace WatStudios.DeepestDungeon.Animation
{
    /// <summary>
    /// Plays a Audio that shall be on enter of a animation state
    /// </summary>
    public class AnimationBasedAudio : StateMachineBehaviour
    {
        #region Private Fields
        //private GameObject _soundEmitter;
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [Header("Audio On Animation Start")]
        [SerializeField] private AudioCollection _audioCollectionAtStart;
        [SerializeField] private int _bankForStart;
        [SerializeField] private bool _playLoopedSourceAtStart;
        [SerializeField] private bool _killLoopedSourceAtStart;

        [Header("Audio On Animation End")]
        [SerializeField] private AudioCollection _audioCollectionAtEnd;
        [SerializeField] private int _bankForEnd;
        [SerializeField] private bool _playLoopedSourceAtEnd;
        [SerializeField] private bool _killLoopedSourceAtEnd;

        [Header("Sound Emitter")]
        //[SerializeField] private GameObject _soundEmitterPrefab;
        //[SerializeField] private float _decayRate = 3.5f;
        //[SerializeField] private float _soundRadius = 10f;
        [Tooltip("Will be set automatically to true if 'Decrease Radius Multiplier' is not 0.")]
        [SerializeField] private bool _keepRunning;
        [SerializeField] [Range(0.0f, 1.0f)] private float _decreaseRadiusMultiplier;
        [Tooltip("Only Works for Sounds that Play on Animation Start.")]
        [SerializeField] private bool _killOnAnimationEnd;
#pragma warning restore 649
        #endregion

        #region StateMachineBehaviour Callbacks
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_audioCollectionAtStart != null)
                if (_playLoopedSourceAtStart)
                    AudioManager.Instance.PlayLoopedSource(animator.gameObject.GetComponent<AudioSource>(), _audioCollectionAtStart, _bankForStart);
                else
                    PlaySound(animator, _audioCollectionAtStart, _bankForStart);

            if (_killLoopedSourceAtStart)
                AudioManager.Instance.StopLoopedSource(animator.gameObject.GetComponent<AudioSource>());
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_audioCollectionAtEnd != null)
                if (_playLoopedSourceAtEnd)
                    AudioManager.Instance.PlayLoopedSource(animator.gameObject.GetComponent<AudioSource>(), _audioCollectionAtEnd, _bankForEnd);
                else
                    PlaySound(animator, _audioCollectionAtEnd, _bankForEnd);

            if (_killLoopedSourceAtEnd)
                AudioManager.Instance.StopLoopedSource(animator.gameObject.GetComponent<AudioSource>());

            //if (_killOnAnimationEnd)
                //Destroy(_soundEmitter);
        } 
        #endregion

        #region Private Methods
        private void PlaySound(Animator animator, AudioCollection collection, int bank)
        {
            //if (_soundEmitterPrefab != null)
            //{
            //    GeneralSoundEmitter soundEmitter = _soundEmitterPrefab.GetComponent<GeneralSoundEmitter>();
            //    soundEmitter.DecayRate = _decayRate;
            //    soundEmitter.SoundRadius = _soundRadius;
            //    soundEmitter.KeepRunning = _keepRunning;
            //    soundEmitter.DecreaseRadiusMultiplier = _decreaseRadiusMultiplier;
            //    _soundEmitter = Instantiate(_soundEmitterPrefab, animator.transform.position, Quaternion.identity) as GameObject;
            //    _soundEmitter.transform.parent = animator.transform;
            //}

            if (AudioManager.Instance != null || _audioCollectionAtStart != null)
            {

                AudioManager.Instance.PlayOneShotSound(collection.AudioGroup,
                                                        collection[bank],
                                                        animator.transform.position,
                                                        collection.Volume,
                                                        collection.SpatialBlend,
                                                        collection.MaxDistance,
                                                        collection.Priority);
            }
        }
        #endregion
    }
}
