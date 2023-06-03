using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class Magazine
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private int _maxValue;
        [SerializeField] private float _reloadTime;
#pragma warning restore 649
        #endregion

        #region Properties
        public int Value { get; private set; }
        public int MaxValue { get => _maxValue; }
        public float ReloadTime => _reloadTime;
        public bool IsReloading { get; set; }
        #endregion
        /// <summary>
        /// Constructor for Magazine
        /// </summary>
        /// <param name="capacity">Capacity of magazine</param>
        /// <param name="reloadTime">Time needed to finish reloading the magazine</param>
        public Magazine(int capacity, float reloadTime)
        {
            _maxValue = capacity;
            _reloadTime = ReloadTime;
            IsReloading = false;
            SetMax();
        }

        /// <summary>
        /// Constructor for Magazine
        /// </summary>
        /// <param name="magObj">Existing Magazine Object</param>
        public Magazine(Magazine magObj)
        {
            _maxValue = magObj._maxValue;
            _reloadTime = magObj._reloadTime;
            IsReloading = false;
            SetMax();
        }

        #region Public Methods
        /// <summary>
        /// Sets the magazine value to zero.
        /// </summary>
        public void SetZero()
        {
            Value = 0;
        }
        /// <summary>
        /// Sets the magazine value to the max value
        /// </summary>
        public void SetMax()
        {
            Value = _maxValue;
        }

        /// <summary>
        /// Reduces the current magizine value by one
        /// </summary>
        public void Reduce()
        {
            if (Value > 0)
                Value--;
        }
        #endregion
    }
}