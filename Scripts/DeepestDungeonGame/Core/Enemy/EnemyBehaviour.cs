using Photon.Pun;
using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic
{
    public abstract class EnemyBehaviour : MonoBehaviourPun
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] private protected EnemyEntity owner;
#pragma warning restore 649
        #endregion

        internal abstract void Start();
        internal abstract void Tick();
    }
}
