using UnityEngine;
using WatStudios.DeepestDungeon.Core.WeaponLogic;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [CreateAssetMenu(fileName = "WeaponBaseStats", menuName = "ScriptableObjects/AttributeSheets/WeaponBaseStats")]
    public class WeaponBaseStats : ScriptableObject
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private string _name;
        [SerializeField] private FiringMode _firingMode;
        [SerializeField] private AmmoType _ammoType;
        [SerializeField] private Sprite _weaponIcon;
#pragma warning restore 649
        #endregion

        #region Attributes
        public AttackDamage Damage;
        public AttackRange Range;
        public AttackSpeed AttackSpeed;
        public Magazine Magazine;
        public OverHeat OverHeat;
        public Spread Spread;
        public Zoom Zoom;
        #endregion

        #region Private Fields
        private bool _enableAiming => Zoom.CanZoom;
        #endregion

        #region Properties
        public string Name => _name;
        public FiringMode FiringMode => _firingMode;
        public AmmoType AmmoType => _ammoType;
        public Sprite WeaponIcon => _weaponIcon;
        #endregion
    }
}
