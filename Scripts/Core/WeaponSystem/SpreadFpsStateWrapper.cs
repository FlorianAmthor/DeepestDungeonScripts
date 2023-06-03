using System;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.PlayerLogic;
using WatStudios.DeepestDungeon.Utility.Editor;

namespace WatStudios.DeepestDungeon.Core.WeaponLogic
{

    [Serializable]
    public class MoveSpeedFpsStateWrapper
    {
        [SerializeField, EnumFlags] private FpsState _fpsState;
        [SerializeField] private SpreadFpsStateData _spreadFpsStateData;

        /// <summary>
        /// The state the SpreadFpsStateData corresponds to
        /// </summary>
        public FpsState FpsState { get => _fpsState; private set => _fpsState = value; }
        /// <summary>
        /// SpreadData
        /// </summary>
        public SpreadFpsStateData SpreadFpsStateData { get => _spreadFpsStateData; private set => _spreadFpsStateData = value; }
    }

    [Serializable]
    public class SpreadFpsStateData
    {
        [SerializeField, Tooltip("Alters min of base spread of the weapon, multiplied with max spread of the weapon"), Range(0, 1f)]  private float _spreadMinOffset;
        [SerializeField, Tooltip("Alters max spread rate of the weapon, multiplied with base value"), Range(0, 2f)] private float _spreadMaxOffset;
        [SerializeField, Tooltip("Alters current spread of the weapon, multiplied with base value"), Range(0, 10f)] private float _spreadFactorAlteration;

        /// <summary>
        /// Value for new minimum spreadrate in given state
        /// </summary>
        public float SpreadMinOffset { get => _spreadMinOffset; private set => _spreadMinOffset = value; }
        /// <summary>
        /// Factor to multiply new max spreadrate in given state
        /// </summary>
        public float SpreadMaxOffset { get => _spreadMaxOffset; private set => _spreadMaxOffset = value; }
        /// <summary>
        /// Factor for altering the spread gained while shooting in this state
        /// </summary>
        public float SpreadFactorAlteration { get => _spreadFactorAlteration; private set => _spreadFactorAlteration = value; }
    }
}