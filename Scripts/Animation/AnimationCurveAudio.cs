using UnityEngine;
using WatStudios.DeepestDungeon.Audio;

namespace WatStudios.DeepestDungeon.Animation
{
    /// <summary>
    /// The Animation Curve should have one of the 
    /// ComChannelNames. Those ComChannels are needed to ferify 
    /// the right AnimationCurve for this animation and sends 
    /// the sound to the AudioMixer
    /// </summary>
    public class AnimationCurveAudio : StateMachineBehaviour
    {
        #region Private Fields
        private int _previousCommand;
        private AudioManager _audioManager;
        private int _commandChannelHash;
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private ComChannelName _commandChannel = ComChannelName.ComChannel1;
        [SerializeField] private AudioCollection _collection;
        [SerializeField] private CustomCurve _customCurve;
        [SerializeField] private int _audioBank;
#pragma warning restore 649
        #endregion

        #region StateMachineBehaviour Callbacks
        /// <summary>
        /// Called prior to the first frame the
        /// animation assigned to this state.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animStateInfo"></param>
        /// <param name="layerIndex"></param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animStateInfo, int layerIndex)
        {
            _audioManager = AudioManager.Instance;
            _previousCommand = 0;

            if (_commandChannelHash == 0)
                _commandChannelHash = Animator.StringToHash(_commandChannel.ToString());
        }

        /// <summary>
        /// Called by the animation system for each frame  
        /// update of the animation
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animStateInfo"></param>
        /// <param name="layerIndex"></param>
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animStateInfo, int layerIndex)
        {
            // Don't make these sounds if our layer weight is zero
            if (layerIndex != 0 && animator.GetLayerWeight(layerIndex).Equals(0.0f)) return;

            int customCommand = (_customCurve == null) ? 0 : Mathf.FloorToInt(_customCurve.Evaluate(animStateInfo.normalizedTime - (long)animStateInfo.normalizedTime));

            int command;
            if (customCommand != 0)
                command = customCommand;
            else
                command = Mathf.FloorToInt(animator.GetFloat(_commandChannelHash));

            if (_previousCommand != command && command > 0 && _audioManager != null && _collection != null)
            {
                int bank = Mathf.Max(0, Mathf.Min(command - 1, _collection.BankCount - 1));
                _audioManager.PlayOneShotSound(_collection.AudioGroup,
                                            _collection[_audioBank == 0 ? bank : _audioBank],
                                            animator.transform.position,
                                            _collection.Volume,
                                            _collection.SpatialBlend,
                                            _collection.MaxDistance,
                                            _collection.Priority);
            }
            _previousCommand = command;
        }
        #endregion
    }
}