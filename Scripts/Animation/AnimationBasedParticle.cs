using System.Collections.Generic;
using UnityEngine;

public class AnimationBasedParticle : StateMachineBehaviour
{
    #region Private Fields
    private List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    #endregion

    #region Exposed Private Fields
#pragma warning disable 649
    [SerializeField] private ParticleSystem[] referenceParticles;
    [SerializeField] private bool loopParticle;
#pragma warning restore 649
    #endregion

    #region StateMachineBehaviour Callbacks
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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

        foreach (ParticleSystem ps1 in particleSystems)
        {
            var main = ps1.main;

            main.loop = loopParticle;
            ps1.Play();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (ParticleSystem ps in particleSystems)
            ps.Stop();
    }
    #endregion
}
