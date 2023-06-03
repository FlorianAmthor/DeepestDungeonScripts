using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class OverHeat
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private float _overheatRate;
        [SerializeField] private float _overheatRegenerationRate;
        [SerializeField] private float _overheatCooldownInSeconds;

#pragma warning restore 649
        #endregion

        #region Properties
        public float Value { get; private set; }
        public float CooldownInSeconds => _overheatCooldownInSeconds;
        public bool IsCoolingDown { get; set; }
        #endregion

        /// <summary>
        /// Constructor for OverHeat component
        /// </summary>
        /// <param name="overRate">How fast the weapon is overheating</param>
        /// <param name="overRegenRate">How fast the weapon regenerates overheat</param>
        /// <param name="coolDownInSec">Cooldown when weapon reches 100% overheat</param>
        public OverHeat(float overRate, float overRegenRate, float coolDownInSec)
        {
            _overheatRate = overRate;
            _overheatRegenerationRate = overRegenRate;
            _overheatCooldownInSeconds = coolDownInSec;
            IsCoolingDown = false;
            SetZero();
        }

        /// <summary>
        /// Constructor for OverHeat component
        /// </summary>
        /// <param name="overheatObj">Existing OverHeat Object</param>
        public OverHeat(OverHeat overheatObj)
        {
            _overheatRate = overheatObj._overheatRate;
            _overheatRegenerationRate = overheatObj._overheatRegenerationRate;
            _overheatCooldownInSeconds = overheatObj._overheatCooldownInSeconds;
            IsCoolingDown = false;
            SetZero();
        }

        #region Public Methods
        /// <summary>
        /// Sets the overheat value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }

        /// <summary>
        /// Increases the overheat value and scales ith with <paramref name="dT"/> 
        /// </summary>
        /// <param name="dT">delta Time</param>
        public void Increase(float dT)
        {
            if (Value < 1.0f)
                Value = Mathf.Clamp(Value + _overheatRate * dT, 0, 1.0f);
        }

        /// <summary>
        /// Reduces the overheat value and scales ith with <paramref name="dT"/> 
        /// </summary>
        /// <param name="dT">delta Time</param>
        public void Reduce(float dT)
        {
            if (Value > 0)
                Value = Mathf.Clamp(Value - _overheatRegenerationRate * dT, 0, 1.0f);
        }
        #endregion
    }
}