using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.FSM
{
    [CreateAssetMenu(fileName = "CyberSoldierAttack", menuName = "ScriptableObjects/AI/Actions/CyberSoldier/Attack")]
    public class CyberSoldierAttack : Action
    {
        public override void Act(EnemyEntity enemy)
        {
            enemy.Animator.SetBool("Attack", true);
            enemy.Animator.SetFloat("Forward", 0);
            enemy.Animator.SetFloat("Turn", 0);
            var info = enemy.Animator.GetCurrentAnimatorClipInfo(0);
            float clipLength = info[0].clip.length;
            enemy.Animator.speed = clipLength / (1.0f / enemy.CurrentStats.AttackSpeed.Value);
            enemy.CurrentStats.LastTimeAttacked = Time.time;
            enemy.TargetPlayer.photonView.RPC("TakeDamage", enemy.TargetPlayer.PhotonPlayer, enemy.CurrentStats.AttackDamage.Value, enemy.photonView.ViewID);

        }
    }
}