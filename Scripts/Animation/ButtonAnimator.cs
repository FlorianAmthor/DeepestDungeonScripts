using UnityEngine;

namespace WatStudios.DeepestDungeon.Animation
{
    public class ButtonAnimator : StateMachineBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private AudioSource _audioSource;
#pragma warning restore 649
        #endregion

        #region StateMachineBehaviour Callbacks
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _audioSource.PlayOneShot(_audioClip);
        }
        #endregion
    }
}