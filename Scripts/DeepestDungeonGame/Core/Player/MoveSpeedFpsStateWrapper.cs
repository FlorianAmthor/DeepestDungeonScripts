using System;
using UnityEngine;
using WatStudios.DeepestDungeon.Utility.Editor;

namespace WatStudios.DeepestDungeon.Core.PlayerLogic
{
    [Serializable]
    public class MoveSpeedFpsStateWrapper
    {
        [SerializeField, EnumFlags] private FpsState _fpsState;
        [SerializeField] private MoveSpeedFpsStateData _moveSpeedFpsStateData;

        /// <summary>
        /// The state the SpreadFpsStateData corresponds to
        /// </summary>
        public FpsState FpsState { get => _fpsState; private set => _fpsState = value; }
        /// <summary>
        /// SpreadData
        /// </summary>
        public MoveSpeedFpsStateData MoveSpeedFpsStateData { get => _moveSpeedFpsStateData; private set => _moveSpeedFpsStateData = value; }
    }
}
