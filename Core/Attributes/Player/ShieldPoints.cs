using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class ShieldPoints
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private int _maxValue;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private float _value;
        #endregion

        #region Properties
        public int Value => (int)_value;
        public int MaxValue => _maxValue;
        public float ShieldPercent => _value / _maxValue;
        #endregion

        /// <summary>
        /// Constructor for ShieldPoints
        /// </summary>
        /// <param name="maxShieldPoints">Upper Boundary for Shield points</param>
        public ShieldPoints(int maxShieldPoints)
        {
            _maxValue = maxShieldPoints;
            SetZero();
        }

        /// <summary>
        /// Constructor for ShieldPoints
        /// </summary>
        /// <param name="shieldComp">Existing ShieldPoints Component</param>
        public ShieldPoints(ShieldPoints shieldComp)
        {
            _maxValue = shieldComp.MaxValue;
            SetZero();
        }

        #region Public Methods
        /// <summary>
        /// Sets the shield points value to zero.
        /// </summary>
        public void SetZero()
        {
            _value = 0;
        }
        /// <summary>
        /// Sets the shield points value to the maximum value
        /// </summary>
        public void SetMax()
        {
            _value = _maxValue;
        }
        /// <summary>
        /// Reduces the current shield points value by amount and clamps it between 0 and max value
        /// </summary>
        /// <param name="amount"></param>
        public int Reduce(float amount)
        {           
            _value -= amount;
            if (_value < 0)
            {
                int result = (int)(_value - amount);
                SetZero();
                return result * -1;
            }
            return 0;
        }
        /// <summary>
        /// Reduces the current shield points value by amount and clamps it between 0 and max value
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>The amount under zero after reducing the shield points</returns>
        public int Reduce(int amount)
        {
            _value -= amount;
            if (_value < 0)
            {
                int result = Value * -1;
                SetZero();
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Increases the current shield points value by amount and clamps it between 0 and max value
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>The amount under zero after reducing the shield points</returns>
        public void Increase(float amount)
        {
            _value = Mathf.Clamp(Value + amount, 0, MaxValue);
        }

        /// <summary>
        /// Increases the current shield points value by amount and clamps it between 0 and max value
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>The amount under zero after reducing the shield points</returns>
        public void Increase(int amount)
        {
            _value = Mathf.Clamp(Value + amount, 0, MaxValue);
        }
        #endregion
    }
}