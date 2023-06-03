namespace WatStudios.DeepestDungeon.Core.Attributes
{
    public class PlayerCurrentStats : EntityCurrentStats
    {
        #region Public Fields
        /// <summary>
        /// Multiplies all threat generating actions with this
        /// </summary>
        public ThreatMultiplier ThreatMultiplier;
        /// <summary>
        /// Multiplier final heal amount, 1.0 = no heal mitigation, max = 1.0
        /// </summary>
        public HealEffectivity HealEffectivity;
        /// <summary>
        /// Number of shieldpoints
        /// </summary>
        public ShieldPoints ShieldPoints;
        /// <summary>
        /// Contains flat and percentual damage modifiers
        /// </summary>
        public DamageModifier DamageModifier;
        #endregion

        #region Public Methods

        public PlayerCurrentStats(PlayerBaseStats baseStats) : base(baseStats)
        {
            ThreatMultiplier = new ThreatMultiplier(baseStats.ThreatMultiplier);
            HealEffectivity = new HealEffectivity(baseStats.HealEffectivity);
            ShieldPoints = new ShieldPoints(baseStats.ShieldPoints);
            DamageModifier = new DamageModifier();
        }
        #endregion
    }
}
