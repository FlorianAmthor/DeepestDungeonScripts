using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;
namespace WatStudios.DeepestDungeon.Audio
{
    public class AudioManager : MonoBehaviour
    {
        #region Private Fields
        private static AudioManager _instance;

        private Dictionary<string, TrackInfo> _tracks = new Dictionary<string, TrackInfo>();
        private List<AudioPoolItem> _pool = new List<AudioPoolItem>();
        private Dictionary<ulong, AudioPoolItem> _activePool = new Dictionary<ulong, AudioPoolItem>();
        private List<LayeredAudioSource> _layeredAudio = new List<LayeredAudioSource>();

        private ulong _idGiver = 0;
        private Transform _listenerPos;
        private bool _audiopos = false;
        #endregion

        #region Exposed Private Fields
        [SerializeField] private AudioMixer _mixer = null;
        [SerializeField] private int _maxSounds = 10;
        #endregion

        #region Public Fields
        public static AudioManager Instance { get { return _instance; } }
        public AudioMixer Mixer { get { return _mixer; } }
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if (_instance != null && _instance != this)
            { Destroy(gameObject); }
            else
            {
                // This object must live for the entire application
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }

            // Return if we have no valid mixer reference
            if (!_mixer) { return; }

            // Fetch all the groups in the mixer - These will be our mixers tracks
            AudioMixerGroup[] groups = _mixer.FindMatchingGroups(string.Empty);

            // Create our mixer tracks based on group name (Track -> AudioGroup)
            foreach (AudioMixerGroup group in groups)
            {
                TrackInfo trackInfo = new TrackInfo();
                trackInfo.Name = group.name;
                trackInfo.Group = group;
                trackInfo.TrackFader = null;
                _tracks[group.name] = trackInfo;
            }

            // Generate Pool
            for (int i = 0; i < _maxSounds; i++)
            {
                // Create GameObject and assigned AudioSource and Parent
                GameObject go = new GameObject("Pool Item");
                AudioSource audioSource = go.AddComponent<AudioSource>();
                go.transform.parent = transform;

                // Create and configure Pool Item
                AudioPoolItem poolItem = new AudioPoolItem();
                poolItem.GameObject = go;
                poolItem.AudioSource = audioSource;
                poolItem.Transform = go.transform;
                poolItem.Playing = false;
                go.SetActive(false);
                _pool.Add(poolItem);

            }
        }

        private void Update()
        {
            if (NetworkPlayer.LocalPlayerInstance != null && _audiopos == false)
            {
                _listenerPos = NetworkPlayer.LocalPlayerInstance.transform;
                _audiopos = true;
            }
            else
            {
                _audiopos = false;
            }
            // Update any layered audio sources
            foreach (LayeredAudioSource las in _layeredAudio)
            {
                if (las != null) { las.Update(); }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// A new scene has just been loaded so we need to find the listener
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (NetworkPlayer.LocalPlayerInstance == null) { return; }
            _listenerPos = NetworkPlayer.LocalPlayerInstance.transform;

            _listenerPos = FindObjectOfType<AudioListener>().transform;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the volume of the AudioMixerGroup assign to the passed track.
        /// AudioMixerGroup MUST expose its volume variable to script for this to
        /// work and the variable MUST be the same as the name of the group
        /// </summary>
        /// 
        public float GetTrackVolume(string track)
        {
            TrackInfo trackInfo;
            if (_tracks.TryGetValue(track, out trackInfo))
            {
                float volume;
                _mixer.GetFloat(track, out volume);
                return volume;
            }

            return float.MinValue;
        }

        /// <summary>
        ///  Gets the Mixer Group by Name
        /// </summary>
        /// <param name="name">Name of the Mixergroup to get</param>
        /// <returns></returns>
        public AudioMixerGroup GetAudioGroupFromTrackName(string name)
        {
            TrackInfo ti;
            if (_tracks.TryGetValue(name, out ti))
            {
                return ti.Group;
            }

            return null;
        }

        /// <summary>
        /// Sets the volume of the AudioMixerGroup assigned to the passed track.
        /// AudioMixerGroup MUST expose its volume variable to script for this to
        /// work and the variable MUST be the same as the name of the group
        /// If a fade time is given a coroutine will be used to perform the fade
        /// </summary>
        /// <param name="track"></param>
        /// <param name="volume"></param>
        /// <param name="fadeTime"></param>
        public void SetTrackVolume(string track, float volume, float fadeTime = 0.0f)
        {
            if (!_mixer) { return; }
            TrackInfo trackInfo;
            if (_tracks.TryGetValue(track, out trackInfo))
            {
                // Stop any coroutine that might be in the middle of fading this track
                if (trackInfo.TrackFader != null) { StopCoroutine(trackInfo.TrackFader); }

                if (fadeTime == 0.0f)
                { _mixer.SetFloat(track, volume); }
                else
                {
                    trackInfo.TrackFader = SetTrackVolumeInternal(track, volume, fadeTime);
                    StartCoroutine(trackInfo.TrackFader);
                }
            }

        }

        /// <summary>
        /// Scores the priority of the sound and search for an unused pool item
        /// to use as the audio source. If one is not available an audio source
        /// with a lower priority will be killed and reused
        /// </summary>
        /// <param name="track"></param>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        /// <param name="volume"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public ulong PlayOneShotSound(string track, AudioClip clip, Vector3 position, float volume, float spatialBlend, float maxDistance, int priority = 128)
        {
            // Do nothing if track does not exist, clip is null or volume is zero
            if (!_tracks.ContainsKey(track) || clip == null || volume.Equals(0.0f)) { return 0; }

            float unimportance = (_listenerPos.position - position).sqrMagnitude / Mathf.Max(1, priority);

            int leastImportantIndex = -1;
            float leastImportanceValue = float.MaxValue;

            // Find an available audio source to use
            for (int i = 0; i < _pool.Count; i++)
            {
                AudioPoolItem poolItem = _pool[i];

                // Is this source available
                if (!poolItem.Playing)
                { return ConfigurePoolObject(i, track, clip, position, volume, spatialBlend, maxDistance, unimportance); }
                else
                // We have a pool item that is less important than the one we are going to play
                if (poolItem.Unimportance > leastImportanceValue)
                {
                    // Record the least important sound we have found so far
                    // as a candidate to relace with our new sound request
                    leastImportanceValue = poolItem.Unimportance;
                    leastImportantIndex = i;
                }
            }

            // If we get here all sounds are being used but we know the least important sound currently being
            // played so if it is less important than our sound request then use replace it
            if (leastImportanceValue > unimportance)
                return ConfigurePoolObject(leastImportantIndex, track, clip, position, volume, spatialBlend, maxDistance, unimportance);

            // Could not be played (no sound in the pool available)
            return 0;
        }

        /// <summary>
        /// Queue up a one shot sound to be played after a number of seconds
        /// </summary>
        /// <param name="track"></param>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        /// <param name="volume"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="duration"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IEnumerator PlayOneShotSoundDelayed(string track, AudioClip clip, Vector3 position, float volume, float spatialBlend, float maxDistance, float duration, int priority = 128)
        {
            yield return new WaitForSeconds(duration);
            PlayOneShotSound(track, clip, position, volume, spatialBlend, maxDistance, priority);
        }

        public ILayeredAudioSource RegisterLayeredAudioSource(AudioSource source, int layers)
        {
            if (source != null && layers > 0)
            {
                // First check it doesn't exist already and if so just return the source
                for (int i = 0; i < _layeredAudio.Count; i++)
                {
                    LayeredAudioSource item = _layeredAudio[i];
                    if (item != null)
                    {
                        if (item.audioSource == source)
                        {
                            return item;
                        }
                    }
                }
                // Create a new layered audio item and add it to the managed list
                LayeredAudioSource newLayeredAudio = new LayeredAudioSource(source, layers);
                _layeredAudio.Add(newLayeredAudio);

                return newLayeredAudio;
            }
            return null;
        }

        public void UnregisterLayeredAudioSource(ILayeredAudioSource source)
        {
            _layeredAudio.Remove((LayeredAudioSource)source);
        }

        /// <summary>
        /// UnregisterLayeredAudioSource (Overload)
        /// </summary>
        /// <param name="source"></param>
        public void UnregisterLayeredAudioSource(AudioSource source)
        {
            for (int i = 0; i < _layeredAudio.Count; i++)
            {
                LayeredAudioSource item = _layeredAudio[i];
                if (item != null)
                {
                    if (item.audioSource == source)
                    {
                        _layeredAudio.Remove(item);
                        return;
                    }
                }
            }
        }

        public void PlayLoopedSource(AudioSource source, AudioCollection collection, int bank)
        {
            source.outputAudioMixerGroup = Instance.GetAudioGroupFromTrackName(collection.AudioGroup);
            source.clip = collection[bank];
            source.volume = collection.Volume;
            source.spatialBlend = collection.SpatialBlend;
            source.maxDistance = collection.MaxDistance;
            source.priority = collection.Priority;
            source.loop = true;
            source.Play();
        }

        public void StopLoopedSource(AudioSource source)
        {
            source.Stop();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Used by SetTrackVolume to implement a fade between volumes of a track
        ///	over time.
        /// </summary>
        /// <param name="track"></param>
        /// <param name="volume"></param>
        /// <param name="fadeTime"></param>
        /// <returns></returns>
        protected IEnumerator SetTrackVolumeInternal(string track, float volume, float fadeTime)
        {
            float startVolume = 0.0f;
            float timer = 0.0f;
            _mixer.GetFloat(track, out startVolume);

            while (timer < fadeTime)
            {
                timer += Time.unscaledDeltaTime;
                _mixer.SetFloat(track, Mathf.Lerp(startVolume, volume, timer / fadeTime));
                yield return null;
            }

            _mixer.SetFloat(track, volume);
        }

        /// <summary>
        /// Used internally to configure a pool object
        /// </summary>
        /// <param name="poolIndex"></param>
        /// <param name="track"></param>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        /// <param name="volume"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="unimportance"></param>
        /// <returns></returns>
        protected ulong ConfigurePoolObject(int poolIndex, string track, AudioClip clip, Vector3 position, float volume, float spatialBlend, float maxDistance, float unimportance)
        {
            // If poolIndex is out of range abort request
            if (poolIndex < 0 || poolIndex >= _pool.Count) { return 0; }

            // Get the pool item
            AudioPoolItem poolItem = _pool[poolIndex];

            // Generate new ID so we can stop it later if we want to
            _idGiver++;

            // Configure the audio source's position and colume
            AudioSource source = poolItem.AudioSource;
            source.clip = clip;
            source.volume = volume;
            source.spatialBlend = spatialBlend;
            source.maxDistance = maxDistance;

            // Assign to requested audio group/track
            source.outputAudioMixerGroup = _tracks[track].Group;

            // Position source at requested position
            source.transform.position = position;

            // Enable GameObject and record that it is now playing
            poolItem.Playing = true;
            poolItem.Unimportance = unimportance;
            poolItem.ID = _idGiver;
            poolItem.GameObject.SetActive(true);
            source.Play();
            poolItem.Coroutine = StopSoundDelayed(_idGiver, source.clip.length);
            StartCoroutine(poolItem.Coroutine);

            // Add this sound to our active pool with its unique id
            _activePool[_idGiver] = poolItem;

            // Return the id to the caller
            return _idGiver;
        }

        /// <summary>
        /// Stop a one shot sound from playing after a number of seconds
        /// </summary>
        /// <param name="id"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected IEnumerator StopSoundDelayed(ulong id, float duration)
        {
            yield return new WaitForSeconds(duration);
            AudioPoolItem activeSound;

            // If this if exists in our active pool
            if (_activePool.TryGetValue(id, out activeSound))
            {
                activeSound.AudioSource.Stop();
                activeSound.AudioSource.clip = null;
                activeSound.GameObject.SetActive(false);
                _activePool.Remove(id);

                // Make it available again
                activeSound.Playing = false;
            }
        }

        #endregion
    }
}
