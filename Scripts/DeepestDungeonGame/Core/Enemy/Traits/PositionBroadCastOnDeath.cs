using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic
{
    [CreateAssetMenu(fileName = "PositionBroadCastOnDeath", menuName = "ScriptableObjects/Enemy Traits/PositionBroadCastOnDeath")]
    public class PositionBroadCastOnDeath : EnemyTrait
    {
        #region Private Exposed Fields
#pragma warning disable 649
        [SerializeField] private float _radius;
#pragma warning restore 649
        #endregion

        public override void Execute(EnemyEntity enemy)
        {
            var cols = Physics.OverlapSphere(enemy.transform.position, _radius, 1 << 14);
            foreach (var col in cols)
            {
                col.GetComponent<EnemyEntity>().SendMessage("OnDeathBroadCast", enemy.transform.position);
            }
        }
    }
}
