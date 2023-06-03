using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Utiliy.Areas
{
    [Serializable]
    public class SphereArea : AreaOfEffect
    {
        #region Private Exposed Fields
#pragma warning disable 649
        [SerializeField] private float _radius;
#pragma warning restore 649
        #endregion

        public override Collider[] GetCollidingObjects(Vector3 origin, Vector3 direction, LayerMask layer)
        {
            return Physics.OverlapSphere(origin, _radius, layer);
        }
    }
}