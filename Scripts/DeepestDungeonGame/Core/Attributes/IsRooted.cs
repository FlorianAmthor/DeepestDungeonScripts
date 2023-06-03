using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{   
    public class IsRooted
    {
        private Duration _duration;

        #region Properties
        public bool Value { get; set; }
        #endregion

        /// <summary>
        /// Constructor for IsRooted
        /// </summary>
        /// <param name="isRooted"></param>
        public IsRooted(bool isRooted)
        {
            Value = isRooted;
        }

        /// <summary>
        /// Constructor for IsRooted
        /// </summary>
        /// <param name="isRootedObj">Existing IsRooted Object</param>
        public IsRooted(IsRooted isRootedObj)
        {
            Value = isRootedObj.Value;
        }

        public void StartRoot(float duration)
        {
            _duration.BaseValue = duration;
            Value = true;
        }

        public void Tick()
        {
            if (Value)
                _duration.Reduce();
            if (_duration.Value <= 0)
                Value = false;
        }
    }
}