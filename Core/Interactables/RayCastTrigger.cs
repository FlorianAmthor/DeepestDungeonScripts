using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Interactables
{
    /// <summary>
    /// This Class is a helper class for RayCasts and for IInteractable interface to be able to trigger the OnTrigger methods of a collider.
    /// </summary>
    public static class RayCastTrigger
    {
        private static bool _prevRayCastHitTrigger;
        private static bool _rayCastHitTrigger;
        private static RaycastHit _interactiveRayCastHit;

        /// <summary>
        /// This method does a raycast and activates the OnTrigger methods of a hit collider accordingly.
        /// </summary>
        /// <param name="position">The starting point of the ray in world coordinates.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="hitInfo">If true is returned, hitInfo will contain more information about where the collider was hit.</param>
        /// <param name="maxRange">The max distance the ray should check for collisions.</param>
        /// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
        /// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
        /// <param name="sender">The collider of the sender of the Raycast to forward to the OnTrigger method</param>
        /// <returns></returns>
        public static bool InteractableRayCast(Vector3 position, Vector3 direction, out RaycastHit hitInfo, float maxRange, int layerMask, QueryTriggerInteraction queryTriggerInteraction, Collider sender)
        {

            _prevRayCastHitTrigger = _rayCastHitTrigger;
            _rayCastHitTrigger = Physics.Raycast(position, direction, out hitInfo, maxRange, layerMask, queryTriggerInteraction);
            if (_rayCastHitTrigger)
            {
                if (!_prevRayCastHitTrigger)
                {
                    hitInfo.collider.SendMessage("OnTriggerEnter", sender);
                    _interactiveRayCastHit = hitInfo;
                }
                else
                {
                    hitInfo.collider.SendMessage("OnTriggerStay", sender);
                    _interactiveRayCastHit = hitInfo;
                }
                _rayCastHitTrigger = true;
                if (Input.GetButtonDown("Interact"))
                {
                    var interactable = hitInfo.collider.GetComponent<Interactable>();
                    if (interactable.CanInteract)
                        interactable.Interact(sender.gameObject);
                }
            }
            else
            {
                if (_prevRayCastHitTrigger)
                {
                    try
                    {
                        _interactiveRayCastHit.collider.SendMessage("OnTriggerExit", sender);
                    }
                    catch (System.Exception) { }
                }
            }
            return _rayCastHitTrigger;
        }
    }
}
