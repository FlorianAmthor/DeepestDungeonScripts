using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Animation
{
    public class RootMotionEnabler : StateMachineBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [Header("At Animation Start")]
        [SerializeField] private bool _enableRootMotionAtStart;
        [SerializeField] private bool _disableRootMotionAtStart;

        [Header("At Animation Exit")]
        [SerializeField] private bool _enableRootMotionAtExit;
        [SerializeField] private bool _disableRootMotionAtExit;
#pragma warning restore 649
        #endregion

        #region StateMachineBehaviour Callbacks
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (_enableRootMotionAtStart)
                animator.applyRootMotion = true;

            if (_disableRootMotionAtStart)
                animator.applyRootMotion = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (_enableRootMotionAtExit)
                animator.applyRootMotion = true;

            if (_disableRootMotionAtExit)
                animator.applyRootMotion = false;
        }
        #endregion
    }
}