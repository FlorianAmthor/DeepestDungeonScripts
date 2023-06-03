using System.Collections.Generic;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.PlayerLogic
{
    [CreateAssetMenu(fileName = "MoveSpeedFpsStateData", menuName = "ScriptableObjects/MoveSpeedData")]
    public class MoveSpeedData : ScriptableObject
    {
        [SerializeField] internal List<MoveSpeedFpsStateWrapper> moveSpeedWrappers;
    }
}
