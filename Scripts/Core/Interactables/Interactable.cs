using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        #region Protected Fields
        protected bool canInteract;
        #endregion

        #region Properties
        public bool CanInteract { get => canInteract; }
        #endregion

        private void Start()
        {
            canInteract = true;
        }

        #region Public Methods
        public abstract void Interact(GameObject interactor);
        #endregion
    }
}
