namespace WatStudios.DeepestDungeon.Core.Attributes
{
    public class EnemyCurrentStats : EntityCurrentStats
    {
        #region Attributes
        public AwarenessRadius AwarenessRadius;
        public AttackDamage AttackDamage;
        public AttackRange AttackRange;
        public AttackSpeed AttackSpeed;
        public float LastTimeAttacked { get; set; }
        #endregion

        public EnemyCurrentStats(EnemyBaseStats baseStats) : base(baseStats)
        {
            AwarenessRadius = new AwarenessRadius(baseStats.AwarenessRadius);
            AttackDamage = new AttackDamage(baseStats.AttackDamage);
            AttackRange = new AttackRange(baseStats.AttackRange);
            AttackSpeed = new AttackSpeed(baseStats.AttackSpeed);
            LastTimeAttacked = 0;
        }
    }
}
