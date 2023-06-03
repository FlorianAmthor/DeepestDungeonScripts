using UnityEngine;

namespace WatStudios.DeepestDungeon.Animation
{
    #region Sub Classes
    [System.Serializable]
    public class FloatPara
    {
        public string ParaName;
        public float ParaValue;
    }

    [System.Serializable]
    public class IntPara
    {
        public string ParaName;
        public int ParaValue;
    }

    [System.Serializable]
    public class BoolPara
    {
        public string ParaName;
        public bool ParaValue;
    }

    [System.Serializable]
    public class TriggerPara
    {
        public string ParaName;
    }
    #endregion

    public class ChangeParameters : StateMachineBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [Header("On Animation Start")]
        [SerializeField] private FloatPara[] ChangeFloatParameterOnStart;
        [SerializeField] private IntPara[] ChangeIntParameterOnStart;
        [SerializeField] private BoolPara[] ChangeBoolParameterOnStart;
        [SerializeField] private TriggerPara[] ChangeTriggerParameterOnStart;

        [Header("On Animation Exit")]
        [SerializeField] private FloatPara[] ChangeFloatParameterOnExit;
        [SerializeField] private IntPara[] ChangeIntParameterOnExit;
        [SerializeField] private BoolPara[] ChangeBoolParameterOnExit;
        [SerializeField] private TriggerPara[] ChangeTriggerParameterOnExit;
#pragma warning restore 649
        #endregion

        #region StateMachineBehaviour Callbacks
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (ChangeFloatParameterOnStart.Length != 0)
                foreach (FloatPara para in ChangeFloatParameterOnStart)
                    animator.SetFloat(para.ParaName, para.ParaValue);

            if (ChangeIntParameterOnStart.Length != 0)
                foreach (IntPara para in ChangeIntParameterOnStart)
                    animator.SetInteger(para.ParaName, para.ParaValue);

            if (ChangeBoolParameterOnStart.Length != 0)
                foreach (BoolPara para in ChangeBoolParameterOnStart)
                    animator.SetBool(para.ParaName, para.ParaValue);

            if (ChangeTriggerParameterOnStart.Length != 0)
                foreach (TriggerPara para in ChangeTriggerParameterOnStart)
                    animator.SetTrigger(para.ParaName);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (ChangeFloatParameterOnExit.Length != 0)
                foreach (FloatPara para in ChangeFloatParameterOnExit)
                    animator.SetFloat(para.ParaName, para.ParaValue);

            if (ChangeIntParameterOnExit.Length != 0)
                foreach (IntPara para in ChangeIntParameterOnExit)
                    animator.SetInteger(para.ParaName, para.ParaValue);

            if (ChangeBoolParameterOnExit.Length != 0)
                foreach (BoolPara para in ChangeBoolParameterOnExit)
                    animator.SetBool(para.ParaName, para.ParaValue);

            if (ChangeTriggerParameterOnExit.Length != 0)
                foreach (TriggerPara para in ChangeTriggerParameterOnExit)
                    animator.SetTrigger(para.ParaName);
        }
        #endregion
    }
}