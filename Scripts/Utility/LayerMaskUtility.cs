using UnityEngine;

namespace WatStudios.DeepestDungeon.Utility
{
    public static class LayerMaskUtility
    {
        public static bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }
    }
}