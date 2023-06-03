using System.Collections;
using UnityEngine;
using WatStudios.DeepestDungeon.Audio;
using WatStudios.DeepestDungeon.Messaging;

namespace WatStudios.DeepestDungeon.Particles
{
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(AudioSource))]
    public class ParticleBasedAudio : MonoBehaviour
    {
        #region Private Fields
        //private GameObject _soundEmitter;
        private ParticleSystem _particleSystem;
        private AudioSource _audioSource;
        private bool _isShooting;

        private bool _played;
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [Header("Audio On Particle Play")]
        [Tooltip("If not selected it will play o")]
        [SerializeField] private bool _weaponDependent;
        [Tooltip("If particle is looped this will be automatically set true.")]
        [SerializeField] private bool _loopSound;
        [SerializeField] private AudioCollection _audioCollection;
        [SerializeField] private int _audioBank;

        [Header("Additions for Loop Sounds")]
        [SerializeField] private bool _startingSound;
        [SerializeField] private int _startingAudioBank;
        [SerializeField] private bool _endingSound;
        [SerializeField] private int _endingAudioBank;

        //[Header("Sound Emitter")]
        //[SerializeField] private GameObject _soundEmitterPrefab;
        //[SerializeField] private float _decayRate = 3.5f;
        //[SerializeField] private float _soundRadius = 10f;
        //[Tooltip("Will be set automatically to true if 'Decrease Radius Multiplier' is not 0.")]
        //[SerializeField] private bool _keepRunning;
        //[SerializeField] [Range(0.0f, 1.0f)] private float _decreaseRadiusMultiplier;
#pragma warning restore 649
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            Init();
            if (!_weaponDependent)
            {
                if (!_played)
                {
                    if (_loopSound)
                    {
                        if (_startingSound)
                            PlayLoopedWithStartSound();
                        else
                            PlayLoopedSound();
                    }
                    else
                        PlaySoundOnce();

                    _played = true;
                }
                else if (!_particleSystem.isEmitting && _played)
                {
                    if (_loopSound)
                    {
                        AudioManager.Instance.StopLoopedSource(_audioSource);
                        if (_endingSound)
                            PlayEndingSound();
                    }

                    //played = false;
                }
            }
            else if (_weaponDependent && _isShooting)
            {
                if (_loopSound)
                {
                    if (_particleSystem.isPlaying && !_played)
                    {
                        if (_startingSound)
                            PlayLoopedWithStartSound();
                        else
                            PlayLoopedSound();
                        _played = true;
                    }
                }
                else
                    PlaySoundOnce();

                _isShooting = false;
            }
            else if (_weaponDependent && !_isShooting)
            {
                if (!_particleSystem.isEmitting && _played)
                {
                    if (_loopSound)
                    {
                        AudioManager.Instance.StopLoopedSource(_audioSource);
                        if (_endingSound)
                            PlayEndingSound();
                        _played = false;
                    }
                }
            }

            if (!_particleSystem.isEmitting && _loopSound)
            {
                AudioManager.Instance.StopLoopedSource(_audioSource);
                if (_endingSound)
                    PlayEndingSound();
            }
        }

        private void OnEnable()
        {
            MessageHub.Subscribe(MessageType.PlayWeaponSound, OnPlayWeaponSound, ActionExecutionScope.Default);
        }

        private void OnDisable()
        {
            MessageHub.Unsubscribe(MessageType.PlayWeaponSound, OnPlayWeaponSound);
        }
        #endregion

        #region Messaging Callbacks
        private void OnPlayWeaponSound(Message msg)
        {
            _isShooting = (bool)msg.Data[0];
        }
        #endregion

        #region Private Methods
        private void Init()
        {
            if (_particleSystem == null)
                _particleSystem = GetComponent<ParticleSystem>();

            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
            if (_particleSystem.main.loop)
                _loopSound = true;
        }

        private void PlaySoundOnce()
        {
            //HandleSoundemitter();

            if (AudioManager.Instance != null && _audioCollection != null)
            {
                AudioManager.Instance.PlayOneShotSound(_audioCollection.AudioGroup,
                                                       _audioCollection[_audioBank],
                                                       transform.position,
                                                       _audioCollection.Volume,
                                                       _audioCollection.SpatialBlend,
                                                       _audioCollection.MaxDistance,
                                                       _audioCollection.Priority);
            }
        }

        private void PlayLoopedWithStartSound()
        {
            _audioSource.outputAudioMixerGroup = AudioManager.Instance.GetAudioGroupFromTrackName(_audioCollection.AudioGroup);
            _audioSource.clip = _audioCollection[_startingAudioBank];
            _audioSource.volume = _audioCollection.Volume;
            _audioSource.spatialBlend = _audioCollection.SpatialBlend;
            _audioSource.maxDistance = _audioCollection.MaxDistance;
            _audioSource.priority = _audioCollection.Priority;
            _audioSource.loop = false;
            _audioSource.Play();

            StartCoroutine(DelayLooped(_audioSource.clip.length));
        }

        private IEnumerator DelayLooped(float time)
        {
            yield return new WaitForSeconds(time);

            PlayLoopedSound();
        }

        private void PlayLoopedSound()
        {
            //HandleSoundemitter();

            if (AudioManager.Instance != null && _audioCollection != null)
            {
                AudioManager.Instance.PlayLoopedSource(_audioSource, _audioCollection, _audioBank);
            }
        }

        private void PlayEndingSound()
        {
            _audioSource.outputAudioMixerGroup = AudioManager.Instance.GetAudioGroupFromTrackName(_audioCollection.AudioGroup);
            _audioSource.clip = _audioCollection[_endingAudioBank];
            _audioSource.volume = _audioCollection.Volume;
            _audioSource.spatialBlend = _audioCollection.SpatialBlend;
            _audioSource.maxDistance = _audioCollection.MaxDistance;
            _audioSource.priority = _audioCollection.Priority;
            _audioSource.loop = false;
            _audioSource.Play();

            StartCoroutine(EmptyClip(_audioSource.clip.length));
        }

        private IEnumerator EmptyClip(float time)
        {
            yield return new WaitForSeconds(time);

            _audioSource.Stop();
            _audioSource.clip = null;
        }

        //private void HandleSoundemitter()
        //{
        //    if (_soundEmitterPrefab != null)
        //    {
        //        GeneralSoundEmitter emitter = _soundEmitterPrefab.GetComponent<GeneralSoundEmitter>();
        //        emitter.DecayRate = _decayRate;
        //        emitter.SoundRadius = _soundRadius;
        //        emitter.KeepRunning = _keepRunning;
        //        emitter.DecreaseRadiusMultiplier = _decreaseRadiusMultiplier;
        //        _soundEmitter = Instantiate(_soundEmitterPrefab, transform.position, Quaternion.identity) as GameObject;
        //        _soundEmitter.transform.parent = transform;
        //    }
        //}
        #endregion
    }
}