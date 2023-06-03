using Photon.Pun;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Animation
{
    public class PlayParticle : StateMachineBehaviour
    {
        #region Private Fields
        private bool _playedStart;
        private bool _playedEnd;
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private ParticleSystem _particleSystem;
        [Header("On Animation Start")]
        [SerializeField] private bool _playOnAnimationStart;
        [Tooltip("Time Should not be larger than the clip Time")]
        [SerializeField] private float _delay;
        [Header("On Animation End")]
        [SerializeField] private bool _playOnAnimationEnd;
#pragma warning restore 649
        #endregion

        #region StateMachineBehaviour Callbacks
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_particleSystem == null)
                return;

            if (_playOnAnimationStart && !_playedStart)
            {
                if (_delay == 0)
                {
                    Play(animator);
                    _playedStart = true;
                }
            }

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_particleSystem == null)
                return;

            if (_playOnAnimationStart && !_playedStart)
            {
                if (_delay != 0)
                {
                    if (stateInfo.normalizedTime >= _delay)
                    {
                        Play(animator);
                        _playedStart = true;
                    }
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_particleSystem == null)
                return;

            if (_playOnAnimationEnd && !_playedEnd)
            {
                Play(animator);
                _playedEnd = true;
            }
        }
        #endregion

        #region Private Methods
        private void Play(Animator animator)
        {
            GameObject particle = PhotonNetwork.Instantiate("Particles/Animation/" + _particleSystem.name, animator.transform.position, animator.transform.rotation) as GameObject;
            particle.transform.parent = animator.transform;
            particle.GetComponent<ParticleSystem>().Play();
        }
        #endregion
    }
}
