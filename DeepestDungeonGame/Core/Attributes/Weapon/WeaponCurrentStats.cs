using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    public class WeaponCurrentStats
    {
        #region Attributes
        public AttackDamage Damage;
        public AttackRange Range;
        public AttackSpeed AttackSpeed;
        public Magazine Magazine;
        public OverHeat OverHeat;
        public Spread Spread;
        public Zoom Zoom;
        #endregion

        #region Properties
        public bool EnableAming { get; set; }
        public float LastFired { get; set; }
        public float DamagePerSecond { get => CalcDps(); }
        #endregion

        public WeaponCurrentStats(WeaponBaseStats baseStats)
        {
            Damage = new AttackDamage(baseStats.Damage);
            Range = new AttackRange(baseStats.Range);
            AttackSpeed = new AttackSpeed(baseStats.AttackSpeed);
            Magazine = new Magazine(baseStats.Magazine);
            OverHeat = new OverHeat(baseStats.OverHeat);
            Spread = new Spread(baseStats.Spread);
            Zoom = new Zoom(baseStats.Zoom);

            EnableAming = false;
            LastFired = Time.time;
        }

        #region Private Methods
        private float CalcDps()
        {
            float time = Magazine.ReloadTime + (Magazine.MaxValue / (1f / AttackSpeed.Value));
            float damage = Magazine.MaxValue * Damage.Value;
            return damage / time;
        }
        #endregion
    }
}