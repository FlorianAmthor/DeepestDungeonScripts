using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    public class DamageModifier
    {

        #region Properties
        public int FlatValue { get; private set; }
        public float PercentualValue { get; private set; }
        #endregion

        /// <summary>
        /// Constructor for DamageModifier
        /// </summary>
        public DamageModifier()
        {
            FlatValue = 0;
            PercentualValue = 1.0f;
        }

        #region Public Methods
        /// <summary>
        /// Sets the flat modifier value to zero.
        /// </summary>
        public void SetFlatZero()
        {
            FlatValue = 0;
        }
        /// <summary>
        /// Sets the percentual modifier value to zero.
        /// </summary>
        public void SetPercentualOne()
        {
            PercentualValue = 1.0f;
        }
        /// <summary>
        /// Increases the current flat modifier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void IncreaseFlat(int amount)
        {
            FlatValue += amount;
        }
        /// <summary>
        /// Increases the current percentual modifier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void IncreasePercentual(float amount)
        {
            PercentualValue += amount;
        }
        /// <summary>
        /// Reduces the current flat modifier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void ReduceFlat(int amount)
        {
            FlatValue = Mathf.Clamp(FlatValue - amount, 0, FlatValue);
        }
        /// <summary>
        /// Reduces the current percentual modifier value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void ReducePercentual(float amount)
        {
            PercentualValue = Mathf.Clamp(PercentualValue - amount, 0, PercentualValue);
        }
        #endregion
    }
}