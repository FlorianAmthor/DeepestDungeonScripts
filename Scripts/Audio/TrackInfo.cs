using System.Collections;
using UnityEngine.Audio;

namespace WatStudios.DeepestDungeon.Audio
{
    /// <summary>
    /// Wraps an AudioMixerGroup in Unity's AudioMixer. Contains the name of the
    ///	group (which is also its exposed volume paramater), the group itself
    ///	and an IEnumerator for doing track fades over time.
    /// </summary>
    public class TrackInfo
    {
        public string Name = string.Empty;
        public AudioMixerGroup Group = null;
        public IEnumerator TrackFader = null;
    }
}
