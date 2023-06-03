using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class ThreatMultiplier
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private float _baseValue;
#pragma warning restore 649
        #endregion

        #region Properties
        public float Value { get; private set; }
        public float BaseValue { get => _baseValue; }
        #endregion

        /// <summary>
        /// Constructor for ThreathMultiplier
        /// </summary>
        /// <param name="baseThreatMult">Base Value for threat Multiplier</param>
        public ThreatMultiplier(float baseThreatMult)
        {
            _baseValue = baseThreatMult;
            SetBase();
        }

        /// <summary>
        /// Constructor for ThreatMultiplier
        /// </summary>
        /// <param name="threatMultObj">Existing ThreatMultiplier Object</param>
        public ThreatMultiplier(ThreatMultiplier threatMultObj)
        {
            _baseValue = threatMultObj._baseValue;
            SetBase();
        }

        /// <summary>
        /// Sets the threat multiplier value to the base value
        /// </summary>
        public void SetBase()
        {
            Value = _baseValue;
        }
        /// <summary>
        /// Increases the current threat multiplier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(float amount)
        {
            Value += amount;
        }
        /// <summary>
        /// Reduces the current threat multiplier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(float amount)
        {
            Value = Mathf.Clamp(Value - amount, 0, Value);
        }
    }
}