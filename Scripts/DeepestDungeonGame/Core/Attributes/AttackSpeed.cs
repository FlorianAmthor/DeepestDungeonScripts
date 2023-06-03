using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class AttackSpeed
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField, Tooltip("Attacks per second")] private float _baseValue;
#pragma warning restore 649
        #endregion

        #region Properties
        public float Value { get; private set; }
        public float BaseValue { get => _baseValue; }
        #endregion

        /// <summary>
        /// Constructor for AttackSpeed
        /// </summary>
        /// <param name="baseSpeed">Base value for attack speed</param>
        public AttackSpeed(float baseSpeed)
        {
            _baseValue = baseSpeed;
            SetBase();
        }

        /// <summary>
        /// Constructor for AttackSpeed
        /// </summary>
        /// <param name="atkSpdObj">Existing AttackSpeed Object</param>
        public AttackSpeed(AttackSpeed atkSpdObj)
        {
            _baseValue = atkSpdObj.BaseValue;
            SetBase();
        }

        #region Public Methods
        /// <summary>
        /// Sets the attack speed value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        /// <summary>
        /// Sets the attack speed value to the base value
        /// </summary>
        public void SetBase()
        {
            Value = _baseValue;
        }
        /// <summary>
        /// Increases the current attack speed value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(int amount)
        {
            Value += amount;
        }
        /// <summary>
        /// Increases the current attack speed value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(float amount)
        {
            Value += amount;
        }
        /// <summary>
        /// Reduces the current attack speed value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(int amount)
        {
            Value = Mathf.Clamp(Value - amount, 0, Value);
        }
        /// <summary>
        /// Reduces the current attack speed value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(float amount)
        {
            Value = Mathf.Clamp(Value - amount, 0, Value);
        }
        #endregion
    }
}