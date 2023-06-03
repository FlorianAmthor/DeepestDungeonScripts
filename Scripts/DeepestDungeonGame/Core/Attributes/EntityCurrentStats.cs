namespace WatStudios.DeepestDungeon.Core.Attributes
{
    public abstract class EntityCurrentStats
    {
        #region Attributes
        public Health Health;
        public MoveSpeed MoveSpeed;
        public DamageTakenMultiplier DamageTakenMultiplier;
        public IsRooted IsRooted;
        #endregion

        public EntityCurrentStats(EntityBaseStats baseStats)
        {
            Health = new Health(baseStats.Health);
            MoveSpeed = new MoveSpeed(baseStats.MoveSpeed);
            DamageTakenMultiplier = new DamageTakenMultiplier(baseStats.DamageTakenMultiplier);
            IsRooted = new IsRooted(false);
        }
    }
}
