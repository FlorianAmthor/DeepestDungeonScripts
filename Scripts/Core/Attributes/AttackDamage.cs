using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class AttackDamage
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private int _baseValue;
#pragma warning restore 649
        #endregion

        #region Properties
        public int Value { get; private set; }
        public int BaseValue { get => _baseValue; }
        #endregion

        /// <summary>
        /// Constructor for AttackDamage
        /// </summary>
        /// <param name="baseDamage">Base Attack damage value</param>
        public AttackDamage(int baseDamage)
        {
            _baseValue = baseDamage;
            SetBase();
        }

        /// <summary>
        /// Constructor for AttackDamage
        /// </summary>
        /// <param name="adComp">Existing Attack Damage Object</param>
        public AttackDamage(AttackDamage atkDmgObj)
        {
            _baseValue = atkDmgObj.BaseValue;
            SetBase();
        }

        #region Public Methods
        /// <summary>
        /// Sets the attack damage value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        /// <summary>
        /// Sets the attack damage value to the base value
        /// </summary>
        public void SetBase()
        {
            Value = _baseValue;
        }
        /// <summary>
        /// Set the attack damage value to int.MaxValue
        /// </summary>
        public void SetInfinite()
        {
            Value = int.MaxValue;
        }
        #endregion
    }
}