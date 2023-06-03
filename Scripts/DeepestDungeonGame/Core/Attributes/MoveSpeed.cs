using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class MoveSpeed
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private float _baseValue;
#pragma warning restore 649
        #endregion

        #region Properties
        public float Value { get; set; }
        public float BaseValue { get => _baseValue; }
        #endregion

        /// <summary>
        /// Constructor for MoveSpeed
        /// </summary>
        /// <param name="baseSpeed">Base value for MoveSpeed</param>
        public MoveSpeed(float baseSpeed)
        {
            _baseValue = baseSpeed;
            SetBase();
        }

        /// <summary>
        /// Constructor for MoveSpeed
        /// </summary>
        /// <param name="moveSpeedObj">Existing MoveSpeed Object</param>
        public MoveSpeed(MoveSpeed moveSpeedObj)
        {
            _baseValue = moveSpeedObj.BaseValue;
            SetBase();
        }

        #region Public Methods
        /// <summary>
        /// Sets the movespeed value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        /// <summary>
        /// Sets the movespeed value to the base value
        /// </summary>
        public void SetBase()
        {
            Value = _baseValue;
        }
        /// <summary>
        /// Increases the current move speed value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(float amount)
        {
            Value += amount;
        }
        /// <summary>
        /// Increases the current move speed value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(int amount)
        {
            Value += amount;
        }
        /// <summary>
        /// Reduces the current move speed value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(float amount)
        {
            Value = Mathf.Clamp(Value - amount, 0, Value);
        }
        /// <summary>
        /// Reduces the current move speed value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(int amount)
        {
            Value = Mathf.Clamp(Value - amount, 0, Value);
        }
        #endregion
    }
}