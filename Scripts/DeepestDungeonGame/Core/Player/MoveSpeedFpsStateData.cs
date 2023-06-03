using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.PlayerLogic
{
    [Serializable]
    public class MoveSpeedFpsStateData
    {
        [SerializeField, Tooltip("Modifies the Movement speed according to the chosen state"), Range(0, 1f)] private float _stateSpeedModifier;
        [SerializeField, Tooltip("Modifies the Movement speed after the State Speed Modifier has been applied"), Range(0, 1f)] private float _zoomSpeedModifier;

        /// <summary>
        /// Factor to alter the current movement speed
        /// </summary>
        public float SpeedModifier { get => _stateSpeedModifier; private set => _stateSpeedModifier = value; }
        /// <summary>
        /// Factor to alter the current movement speed while zooming, applied after the  state speed mofidier
        /// </summary>
        public float ZoomSpeedModifier { get => _zoomSpeedModifier; private set => _zoomSpeedModifier = value; }
    }
}
