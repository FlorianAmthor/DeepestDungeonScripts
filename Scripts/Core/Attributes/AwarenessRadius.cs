using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class AwarenessRadius
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
        /// Constructor for AwarenessRadius
        /// </summary>
        /// <param name="baseRadius">Basle value for the radius</param>
        public AwarenessRadius(float baseRadius)
        {
            _baseValue = baseRadius;
            SetBase();
        }

        /// <summary>
        /// Constructor for AwarenessRadius
        /// </summary>
        /// <param name="aRadiusObj">Existing AwarenessRadius Object</param>
        public AwarenessRadius(AwarenessRadius aRadiusObj)
        {
            _baseValue = aRadiusObj.BaseValue;
            SetBase();
        }

        #region Public Methods
        /// <summary>
        /// Sets the awareness radius value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        /// <summary>
        /// Sets the awareness radius value to the base value
        /// </summary>
        public void SetBase()
        {
            Value = _baseValue;
        }
        /// <summary>
        /// Increases the current awareness radius value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(float amount)
        {
            Value += amount;
        }
        /// <summary>
        /// Increases the current awareness radius value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(int amount)
        {
            Increase((float)amount);
        }
        /// <summary>
        /// Reduces the current awareness radius value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(float amount)
        {
            Value = Mathf.Clamp(Value - amount, 0, Value);
        }
        /// <summary>
        /// Reduces the current awareness radius value by amount
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(int amount)
        {
            Reduce((float)amount);
        }
        #endregion
    }
}