using System;
using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Utiliy.Areas
{
    [Serializable]
    public class CircleSectorArea : AreaOfEffect
    {
        #region Private Exposed Fields
#pragma warning disable 649
        [SerializeField] private float _angle;
        [SerializeField] private float _radius;
#pragma warning restore 649
        #endregion

        public override Collider[] GetCollidingObjects(Vector3 origin, Vector3 direction, LayerMask layer)
        {
            var cols = Physics.OverlapSphere(origin, _radius, layer);
            float maxAngle = Mathf.Cos(_angle * Mathf.Deg2Rad / 2.0f);
            List<Collider> resultCols = new List<Collider>();

            foreach (var col in cols)
            {
                if (Vector3.Dot(direction, (col.transform.position - origin).normalized) > maxAngle)
                {
                    resultCols.Add(col);
                }
            }
            return resultCols.ToArray();
        }
    }
}