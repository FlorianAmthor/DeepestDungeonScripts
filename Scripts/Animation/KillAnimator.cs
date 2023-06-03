using UnityEngine;

namespace WatStudios.DeepestDungeon.Animation
{
    public class KillAnimator : StateMachineBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private bool _enableRagdoll;
        [SerializeField] private bool _killAllBehaviours;
#pragma warning restore 649
        #endregion

        #region StateMachineBehaviour Callbacks
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            KillIt(animator);
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("exit state death");
        }

        private void KillIt(Animator animator)
        {
            var rBody = animator.GetComponent<Rigidbody>();
            if (rBody)
            {
                if (animator.GetComponent<Rigidbody>().isKinematic)
                    animator.GetComponent<Rigidbody>().isKinematic = false;
            }

            if (_enableRagdoll)
                EnableRagdoll(animator);
            else
            {
                if (rBody)
                    Destroy(animator.GetComponent<Rigidbody>());
                foreach (var col in animator.GetComponents<Collider>())
                {
                    Destroy(col);
                }
            }

            Destroy(animator.GetComponent<AudioSource>());

            if (_killAllBehaviours)
            {
                Behaviour[] bs = animator.GetComponents<Behaviour>();
                foreach (Behaviour b in bs)
                    Destroy(b);
            }
            else
            {
                //Disable on animation end
                //Destroy(animator);
            }
        }
        #endregion

        #region Private Methods
        private void EnableRagdoll(Animator animator)
        {
            Collider[] mainColls = animator.GetComponents<Collider>();
            foreach (Collider coll in mainColls)
                Destroy(coll);

            Destroy(animator.GetComponent<Rigidbody>());

            Rigidbody[] rbs = animator.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                if (rb.isKinematic)
                    rb.isKinematic = false;
            }

            Collider[] colls = animator.GetComponentsInChildren<Collider>();
            foreach (Collider coll in colls)
            {
                if (!coll.enabled)
                    coll.enabled = true;
                if (coll.isTrigger)
                    coll.isTrigger = false;
            }
        }
        #endregion
    }
}