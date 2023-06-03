using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class Health
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private int _maxValue;
        [SerializeField] private float _healthRegenPerSecond;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private float _value;
        #endregion

        #region Properties
        public int Value { get => (int)_value; internal set => _value = Mathf.Clamp(value,0,_maxValue); }
        public int MaxValue { get => _maxValue; }
        public float HealthPercent => _value / _maxValue;
        public float HealthRegenPerSecond { get => _healthRegenPerSecond; }
        #endregion

        /// <summary>
        /// Constructor for Health
        /// </summary>
        /// <param name="maxHealth">Upper boundary for the Health value</param>
        /// <param name="healthPerSecond">Amount of Health regenerated per second</param>
        public Health(int maxHealth, float healthPerSecond)
        {
            _maxValue = maxHealth;
            _healthRegenPerSecond = healthPerSecond;
            SetMax();
        }

        /// <summary>
        /// Constructor for Health
        /// </summary>
        /// <param name="healthObj">Exsiting Health Object</param>
        public Health(Health healthObj)
        {
            _maxValue = healthObj.MaxValue;
            _healthRegenPerSecond = healthObj.HealthRegenPerSecond;
            SetMax();
        }

        #region Public Methods
        /// <summary>
        /// Sets the health value to zero.
        /// </summary>
        public void SetZero()
        {
            _value = 0;
        }
        /// <summary>
        /// Sets the health value to the maximum value
        /// </summary>
        public void SetMax()
        {
            _value = _maxValue;
        }
        /// <summary>
        /// Increases the current health value by amount and clamps it between 0 and max value
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(float amount)
        {
            _value = Mathf.Clamp(_value + amount, 0, _maxValue);
        }
        /// <summary>
        /// Increases the current health value by amount and clamps it between 0 and max value
        /// </summary>
        /// <param name="amount"></param>
        public void Increase(int amount)
        {
            _value = Mathf.Clamp(_value + amount, 0, _maxValue);
        }
        /// <summary>
        /// Reduces the current health value by amount and clamps it between 0 and max value
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(float amount)
        {
            _value = Mathf.Clamp(_value - amount, 0, _maxValue);
        }
        /// <summary>
        /// Reduces the current health value by amount and clamps it between 0 and max value
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(int amount)
        {
            _value = Mathf.Clamp(_value - amount, 0, _maxValue);
        }
        /// <summary>
        /// Regenerates Life according to _healthRegenPerSecond multiplied with <paramref name="deltaTime"/>
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Regenerate(float deltaTime)
        {
            _value = Mathf.Clamp(_value + _healthRegenPerSecond * deltaTime, _value, _maxValue);
        }
        #endregion
    }
}