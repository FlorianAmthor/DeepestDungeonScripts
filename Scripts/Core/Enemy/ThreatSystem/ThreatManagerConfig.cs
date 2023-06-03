using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.ThreatSystem
{
    [CreateAssetMenu(fileName = "ThreatManagerConfig", menuName = "ScriptableObjects/AI/ThreatManagerConfig")]
    public class ThreatManagerConfig : ScriptableObject
    {
        #region Private Exposed Fields
#pragma warning disable 649
        [SerializeField, Tooltip("BaseThreat for every player if he aggros a monster without damaging it")]
        private int _baseThreat;
        [SerializeField, Tooltip("Percentual value of current threat that should be lost per second"), Range(0, 1)]
        private float _percentualThreatLossPerSecond = 0.0f;
#pragma warning restore 649
        #endregion

        #region Properties
        /// <summary>
        /// BaseThreat for every player if he aggros a monster without damaging it
        /// </summary>
        public int BaseThreat => _baseThreat;
        /// <summary>
        /// Percentage Value of Threat that should be lost per second
        /// </summary>
        public float PercentualThreatLossPerSecond => _percentualThreatLossPerSecond;
        #endregion
    }
}
