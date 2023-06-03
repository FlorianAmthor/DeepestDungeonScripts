using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class DamageTakenMultiplier
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField, Range(0.5f, 1.5f)] private float _baseValue;
#pragma warning restore 649
        #endregion

        #region Properties
        public float Value { get; set; }
        public float BaseValue { get => _baseValue; }
        #endregion

        /// <summary>
        /// Constructor for DamageTakenMultiplier
        /// </summary>
        /// <param name="baseMult">Base value for the multiplier</param>
        public DamageTakenMultiplier(float baseMult)
        {
            _baseValue = baseMult;
            SetBase();
        }

        /// <summary>
        /// Constructor for DamageTakenMultiplier
        /// </summary>
        /// <param name="dmgTakenMultObj">Existing DamageTakenMultiplier Object</param>
        public DamageTakenMultiplier(DamageTakenMultiplier dmgTakenMultObj)
        {
            _baseValue = dmgTakenMultObj.BaseValue;
            SetBase();
        }

        #region Public Methods
        /// <summary>
        /// Sets the damage taken multiplier value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        /// <summary>
        /// Sets the damage taken multiplier value to the base value
        /// </summary>
        public void SetBase()
        {
            Value = _baseValue;
        }
        /// <summary>
        /// Increases the current damage taken multiplier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(float amount)
        {
            Value += amount;
        }
        /// <summary>
        /// Increases the current damage taken multiplier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(int amount)
        {
            Increase((float)amount);
        }
        /// <summary>
        /// Reduces the current damage taken multiplier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(float amount)
        {
            Value = Mathf.Clamp(Value - amount, 0, Value);
        }
        /// <summary>
        /// Reduces the current damage taken multiplier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(int amount)
        {
            Reduce((float)amount);
        }
        #endregion
    }
}