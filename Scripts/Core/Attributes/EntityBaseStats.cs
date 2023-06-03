using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    public abstract class EntityBaseStats : ScriptableObject
    {
        #region Exposed Protected Fields
#pragma warning disable 649
        [SerializeField] protected string _name;
#pragma warning restore 649
        #endregion

        #region Properties
        public string Name => _name;
        #endregion

        #region Attributes
        [Header("Attributes")]
        public Health Health;
        public MoveSpeed MoveSpeed;
        public DamageTakenMultiplier DamageTakenMultiplier;
        #endregion
    }
}