using System;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/AttributeSheets/EnemyStats")]
    public class EnemyBaseStats : EntityBaseStats
    {
        #region Attributes
        public AwarenessRadius AwarenessRadius;
        public AttackDamage AttackDamage;
        public AttackRange AttackRange;
        public AttackSpeed AttackSpeed;
        #endregion
    }
}