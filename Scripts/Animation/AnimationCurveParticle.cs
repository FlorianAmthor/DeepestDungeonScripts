using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Animation
{
    public class AnimationCurveParticle : StateMachineBehaviour
    {
        #region Private Fields
        private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
        private bool isPlaying;
        private int _commandChannelHash;
        #endregion

        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private ParticleSystem[] referenceParticles;
        [SerializeField] private bool loopParticle;
        [SerializeField] private ParticleChannelName _commandChannel = ParticleChannelName.ParticleChannel1;
        [SerializeField] private CustomCurve _customCurve;
        //[SerializeField] StringList _layerExclusions;
#pragma warning restore 649
        #endregion

        #region StateMachineBehaviour Callbacks
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            particleSystems = new List<ParticleSystem>();
            ParticleSystem[] ps = animator.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < ps.Length; i++)
            {
                for (int j = 0; j < referenceParticles.Length; j++)
                {
                    if (ps[i].name == referenceParticles[j].name)
                    {
                        particleSystems.Add(ps[i]);
                    }
                }
            }

            if (_commandChannelHash == 0)
                _commandChannelHash = Animator.StringToHash(_commandChannel.ToString());
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Don't make these sounds if our layer weight is zero
            if (layerIndex != 0 && animator.GetLayerWeight(layerIndex).Equals(0.0f)) return;

            int customCommand = (_customCurve == null) ? 0 : Mathf.FloorToInt(_customCurve.Evaluate(stateInfo.normalizedTime - (long)stateInfo.normalizedTime));

            int command;
            if (customCommand != 0)
                command = customCommand;
            else
                command = Mathf.FloorToInt(animator.GetFloat(_commandChannelHash));

            if (command > 0)
            {
                if (!isPlaying)
                {
                    foreach (ParticleSystem ps in particleSystems)
                    {

                        var main = ps.main;

                        main.loop = loopParticle;
                        ps.Play();

                    }

                    isPlaying = true;
                }
            }
            else
            {
                foreach (ParticleSystem ps in particleSystems)
                    ps.Stop();

                isPlaying = false;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (ParticleSystem ps in particleSystems)
                ps.Stop();

            isPlaying = false;
        }
        #endregion
    }
}