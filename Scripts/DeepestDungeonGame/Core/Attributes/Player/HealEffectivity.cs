using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class HealEffectivity
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField, Range(0, 0.1f)] private float _reducePerStack;
        [SerializeField] private float _minEffectivity;
#pragma warning restore 649
        #endregion

        #region Private Fields
        private float _baseValue;
        private int _numOfStacks;
        private int _maxNumOfStacks;
        #endregion

        #region Properties
        public float Value { get => 1.0f - _numOfStacks * _reducePerStack; }
        public int NumOfStacks => _numOfStacks;
        public float LastTimeReduced { get; private set; }
        public float LastTimeIncreased { get; private set; }
        #endregion

        /// <summary>
        /// Constructor for HealEffectivity
        /// </summary>
        /// <param name="reducePerStack">Heal penalty for one stack</param>
        /// <param name="minEffectivity">Lower boundary for Healeffectivity</param>
        /// <param name="incEffectivityTime">Idle time in seconds after which HealEffectitivy is increased again</param>
        public HealEffectivity(float reducePerStack, float minEffectivity, float incEffectivityTime)
        {
            _reducePerStack = reducePerStack;
            _minEffectivity = minEffectivity;
            _maxNumOfStacks = (int)((1.0f - _minEffectivity) / _reducePerStack) + 1;
            LastTimeReduced = 0;
            LastTimeIncreased = 0;
            SetZero();
        }

        /// <summary>
        /// Constructor for HealEffectivity
        /// </summary>
        /// <param name="healEffectObj">Existing HealEffectivity Object</param>
        public HealEffectivity(HealEffectivity healEffectObj)
        {
            _reducePerStack = healEffectObj._reducePerStack;
            _minEffectivity = healEffectObj._minEffectivity;
            _maxNumOfStacks = (int)((1.0f - _minEffectivity) / _reducePerStack) + 1;
            LastTimeReduced = 0;
            LastTimeIncreased = 0;
            SetZero();
        }

        #region Public Methods
        /// <summary>
        /// Sets the number of stacks to zero.
        /// </summary>
        public void SetZero()
        {
            _numOfStacks = 0;
        }
        /// <summary>
        /// Increases HealEffectivity by reducing the current number of stacks by one
        /// </summary>
        /// <param name="amount"></param>
        public void Increase()
        {
            if (_numOfStacks > 0)
            {
                _numOfStacks--;
                LastTimeIncreased = Time.time;
            }

        }
        /// <summary>
        /// Reduces HealEffectivity by increasing the current number of stacks by one
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce()
        {
            if (_numOfStacks < _maxNumOfStacks)
                _numOfStacks++;
            LastTimeReduced = Time.time;
        }
        #endregion
    }
}