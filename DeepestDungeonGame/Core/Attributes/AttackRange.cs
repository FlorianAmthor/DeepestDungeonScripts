using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class AttackRange
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
        /// Constructor for AttackRange
        /// </summary>
        /// <param name="baseRange">Base Attack Range Value</param>
        public AttackRange(float baseRange)
        {
            _baseValue = baseRange;
            SetBase();
        }

        /// <summary>
        /// Constructor for AttackRange
        /// </summary>
        /// <param name="atkRangeObj">Existing AttackRange Object</param>
        public AttackRange(AttackRange atkRangeObj)
        {
            _baseValue = atkRangeObj.BaseValue;
            SetBase();
        }

        #region Public Methods
        /// <summary>
        /// Sets the attack range value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        /// <summary>
        /// Sets the attack range value to the base value
        /// </summary>
        public void SetBase()
        {
            Value = _baseValue;
        }
        /// <summary>
        /// Increases the current attack range value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(int amount)
        {
            Value += amount;
        }
        /// <summary>
        /// Increases the current attack range value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(float amount)
        {
            Value += amount;
        }
        /// <summary>
        /// Reduces the current attack range value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(int amount)
        {
            Value = Mathf.Clamp(Value - amount, 0, Value);
        }
        /// <summary>
        /// Reduces the current attack range value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(float amount)
        {
            Value = Mathf.Clamp(Value - amount, 0, Value);
        }
        #endregion
    }
}