using UnityEngine;

namespace WatStudios.DeepestDungeon.Animation
{
    /// <summary>
    /// A Custom Animation Curve if the Animation does not offer the option, because
    /// the animation is of type .anim and not .fbx
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Animation/Custom Curve", fileName = "Custom Animation Curve")]
    public class CustomCurve : ScriptableObject
    {
        #region Exposed Private Fields
        [SerializeField] AnimationCurve _curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        #endregion

        #region Public Methods
        public float Evaluate(float t)
        {
            return _curve.Evaluate(t);
        }
        #endregion

    }
}
