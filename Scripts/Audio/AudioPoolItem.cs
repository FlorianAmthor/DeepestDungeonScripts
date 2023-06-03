using System.Collections;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Audio
{
    /// <summary>
    /// Describes an audio entity in our pooling system.
    /// </summary>
    public class AudioPoolItem
    {
        public GameObject GameObject = null;
        public Transform Transform = null;
        public AudioSource AudioSource = null;
        public float Unimportance = float.MaxValue;
        public bool Playing = false;
        public IEnumerator Coroutine = null;
        public ulong ID = 0;
    }
}
