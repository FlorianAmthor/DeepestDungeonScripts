using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Audio
{
    /// <summary>
    /// This is a helper class used by the
    /// AudioCollection class. It stores a list
    /// of AudioClips
    /// </summary>
    [System.Serializable]
    public class ClipBank
    {
        public string _name;
        public List<AudioClip> _clips = new List<AudioClip>();
    }

    /// <summary>
    /// Scriptable Object class used to represent
    /// Audio Collection assets.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Audio/Audio Collection", fileName = "New Audio Collection")]
    public class AudioCollection : ScriptableObject
    { 
        #region Exposed Private Fields
        [SerializeField] private string _audioGroup = string.Empty;
        [SerializeField] [Range(0.0f, 1.0f)] private float _volume = 1.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _spatialBlend = 1.0f;
        [SerializeField] private float _maxDistance = 10f;
        [SerializeField] [Range(0, 256)] private int _priority = 128;
        [SerializeField] private List<ClipBank> _audioClipBanks = new List<ClipBank>();
        #endregion

        #region Public Fields
        public string AudioGroup { get { return _audioGroup; } }
        public float Volume { get { return _volume; } }
        public float SpatialBlend { get { return _spatialBlend; } }

        public float MaxDistance { get { return _maxDistance; } }
        public int Priority { get { return _priority; } }
        public int BankCount { get { return _audioClipBanks.Count; } }
        #endregion

        #region Public Methods
        /// <summary>
        /// Allows us to fetch a random audio clip
        /// from the bank specified in the square 
        /// brackets.
        /// 
        /// AudioClip clip = MyCollection[1];
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public AudioClip this[int i]
        {
            get
            {
                // Return if banks don't exist, are empty or the bank index
                // specified is out of range	
                if (_audioClipBanks == null || _audioClipBanks.Count <= i) return null;
                if (_audioClipBanks[i]._clips.Count == 0) return null;

                // Fetch the ClipBank we wish to sample from
                List<AudioClip> clipList = _audioClipBanks[i]._clips;

                // Select random clip from the bank
                AudioClip clip = clipList[Random.Range(0, clipList.Count)];

                // return clip
                return clip;
            }
        }

        /// <summary>
        /// Returns a random audio clip from the 1st
        /// lip bank (Bank[0]);
        /// </summary>
        public AudioClip audioClip
        {
            get
            {
                if (_audioClipBanks == null || _audioClipBanks.Count == 0) return null;
                if (_audioClipBanks[0]._clips.Count == 0) return null;

                List<AudioClip> clipList = _audioClipBanks[0]._clips;
                AudioClip clip = clipList[Random.Range(0, clipList.Count)];
                return clip;
            }
        }
        #endregion

    }
}
