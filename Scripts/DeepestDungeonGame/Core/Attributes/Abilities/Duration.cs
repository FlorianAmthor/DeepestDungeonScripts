using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class Duration
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private float _baseValue;
#pragma warning restore 649
        #endregion

        #region Properties
        public float Value { get; private set; }
        public float BaseValue { get => _baseValue; set => _baseValue = value; }
        #endregion

        /// <summary>
        /// Constructor for Duration
        /// </summary>
        /// <param name="baseValue">Base Duration Value</param>
        public Duration(float baseValue)
        {
            _baseValue = baseValue;
            SetBase();
        }

        /// <summary>
        /// Constructor for Duration
        /// </summary>
        /// <param name="durationObj">Existing Duration Object</param>
        public Duration(Duration durationObj)
        {
            _baseValue = durationObj.BaseValue;
            SetBase();
        }

        #region Public Methods
        /// <summary>
        /// Sets the duration value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        /// <summary>
        /// Sets the duration value to the base value
        /// </summary>
        public void SetBase()
        {
            Value = _baseValue;
        }
        /// <summary>
        /// Reduces the current duration value by amount
        /// </summary>
        public void Reduce()
        {
            Value = Mathf.Clamp(Value - Time.deltaTime, 0, Value);
        }
        #endregion
    }
}