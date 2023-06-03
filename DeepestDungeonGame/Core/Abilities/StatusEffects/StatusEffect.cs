using UnityEngine;
using WatStudios.DeepestDungeon.Core.Attributes;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects
{
    public abstract class StatusEffect : ScriptableObject
    {
        #region Exposed Private Fields
#pragma warning disable 649
        [SerializeField] protected Sprite buffSprite;
        [SerializeField] protected Duration duration;
        [SerializeField] protected bool isBuff;
        [SerializeField] protected bool isStackable;
#pragma warning restore 649
        #endregion

        #region Properties
        public IStatusEntity Source { get; set; }
        public Sprite BuffSprite => buffSprite;
        public Duration Duration => duration;
        public bool IsBuff => isBuff;
        public bool IsStackable => isStackable;
        public virtual int NumOfStacks(IStatusEntity entity)
        {
            return -1;
        }
        public abstract bool IsExpired(IStatusEntity entity);
        #endregion

        //parameter for all entity, create an interface?
        internal abstract void Apply(IStatusEntity entity);
        internal abstract void Undo(IStatusEntity entity);
        internal virtual void Tick(IStatusEntity entity) { }
        internal virtual void Refresh(IStatusEntity entity)
        {
            Apply(entity);
        }
    }
}