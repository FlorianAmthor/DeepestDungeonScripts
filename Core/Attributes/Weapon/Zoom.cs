using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [Serializable]
    public class Zoom
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField]
        private bool _canZoom;
        [SerializeField, Range(0f, 30f)]
        private float _factor;
        [SerializeField, Range(0f, 1f), Tooltip("Will be multiplied with base spread factor. A high low value will increase accuracy while zooming")]
        private float _spreadReduce;
#pragma warning restore 649
        #endregion

        #region Properties
        public bool IsZooming { get; private set; }
        public bool CanZoom => _canZoom;
        public float Factor => _factor;
        public float SpreadReduce => _spreadReduce;
        #endregion

        /// <summary>
        /// Constructor for Zoom
        /// </summary>
        /// <param name="canZoom">Can the player zoom with the weapon</param>
        /// <param name="zoomFactor">How far the player can zoom</param>
        /// <param name="spreadReduce">Value between [0,1], Percentual Reduce to spread factor while zoominf</param>
        public Zoom(bool canZoom, float zoomFactor, float spreadReduce)
        {
            SetZooming(false);
            _canZoom = canZoom;
            _factor = zoomFactor;
            _spreadReduce = spreadReduce;
        }

        /// <summary>
        /// Constructor for Zoom
        /// </summary>
        /// <param name="zoomComp">Existing Zoom Component</param>
        public Zoom(Zoom zoomComp)
        {
            SetZooming(false);
            _canZoom = zoomComp._canZoom;
            _factor = zoomComp._factor;
            _spreadReduce = zoomComp._spreadReduce;
        }

        #region Public Methods
        /// <summary>
        /// Sets the IsZooming Value of the Weapon according to _canZoom
        /// </summary>
        public void SetZooming(bool isZooming)
        {
            if (!_canZoom)
                IsZooming = false;
            else
                IsZooming = isZooming;
        }
        #endregion
    }
}