using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class Spread
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField, Range(0f, 0.5f)] private float _maxValue;
        [SerializeField, Range(0f, 0.01f), Tooltip("Applied spread while shooting")] private float _factor;
        [SerializeField, Range(0f, 0.01f)] private float _spreadRegenerationRate;
#pragma warning restore 649
        #endregion

        #region Properties
        public float Value { get; set; }
        public float IsRegenerating { get; set; }
        public float MaxValue { get => _maxValue; set => _maxValue = value; }
        public float MinValue { get; set; }
        public float Factor { get => _factor; set => _factor = value; }
        public float RegenerationRate { get => _spreadRegenerationRate; }
        #endregion

        /// <summary>
        /// Construcor for Spread Attribute
        /// </summary>
        /// <param name="maxSpread">Maximum Spread of the weapon</param>
        /// <param name="spreadFactor">How fast the weapon is spreading</param>
        /// <param name="spreadRegenRate">How fast weapon aim is regenerating</param>
        public Spread(float maxSpread, float spreadFactor, float spreadRegenRate)
        {
            _maxValue = maxSpread;
            MinValue = 0;
            _factor = spreadFactor;
            _spreadRegenerationRate = spreadRegenRate;
            SetZero();
        }

        /// <summary>
        /// Construcor for Spread Attribute
        /// </summary>
        /// <param name="spreadObj">Existing Spread Object</param>
        public Spread(Spread spreadObj)
        {
            _maxValue = spreadObj._maxValue;
            MinValue = 0;
            _factor = spreadObj._factor;
            _spreadRegenerationRate = spreadObj._spreadRegenerationRate;
            SetZero();
        }

        #region Public Methods
        /// <summary>
        /// Sets the spread value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        #endregion
    }
}