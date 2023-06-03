using UnityEngine;

namespace WatStudios.DeepestDungeon.Audio
{
    /// <summary>
    /// Describes an audio layer in our pooling system.
    /// </summary>
    public class AudioLayer
    {
        public AudioClip Clip = null;
        public AudioCollection Collection = null;
        public int Bank = 0;
        public bool Looping = true;
        public float Time = 0.0f;
        public float Duration = 0.0f;
        public bool Muted = false;
    }
}
