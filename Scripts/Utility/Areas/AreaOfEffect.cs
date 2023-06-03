using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Utiliy.Areas
{
    public abstract class AreaOfEffect
    {
        public abstract Collider[] GetCollidingObjects(Vector3 origin, Vector3 direction, LayerMask layer);
    }
}