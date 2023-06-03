using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic
{
    public abstract class EnemyTrait : ScriptableObject
    {
        #region Protected Exposed Fields
#pragma warning disable 649
        [SerializeField] protected string _name;
#pragma warning restore 649
        #endregion

        #region Properties
        public string Name => _name;
        #endregion

        public abstract void Execute(EnemyEntity enemy);
    }
}
