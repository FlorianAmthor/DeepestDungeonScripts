using UnityEngine;
namespace WatStudios.DeepestDungeon.UI.Menu
{
    public class MenuButton : MonoBehaviour
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private MenuButtonController _menuButtonController;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimatorFunctions _animatorFunctions;
#pragma warning restore 649
        #endregion

        #region Public Methods
        public void OnPointerEnter()
        {
            _animator.SetBool("selected", true);
        }

        public void OnPointerExit()
        {
            _animator.SetBool("selected", false);
            _animator.SetBool("pressed", false);
        }

        public void OnPointerClick()
        {
            _animator.SetBool("pressed", true);
        }
        #endregion
    }
}