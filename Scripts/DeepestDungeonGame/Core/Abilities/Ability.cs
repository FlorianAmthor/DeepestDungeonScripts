using UnityEngine;
using WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects;
using WatStudios.DeepestDungeon.Core.Attributes;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities
{
    public abstract class Ability : ScriptableObject
    {
        #region Exposed Protected Fields
#pragma warning disable 649
        [SerializeField] protected new string name;
        [SerializeField] protected string description;
        [SerializeField] protected AbilityActivation abilityActivation;
        [SerializeField] protected bool useQuickCast;
        [SerializeField] protected bool hasPreview;
        [SerializeField] protected Sprite abilityImage;
        [SerializeField] protected LayerMask afflictedObjects;
        [SerializeField] protected Cooldown cooldown;
        [SerializeField] protected StatusEffect statusEffect;
#pragma warning restore 649
        #endregion

        #region Properties
        public string Name => name;
        public string Description => description;
        public AbilityActivation AbilityActivation => abilityActivation;
        public bool UseQuickCast => useQuickCast;
        public Sprite AbilityImage => abilityImage;
        public Cooldown Cooldown => cooldown;
        public virtual bool IsUsable { get; protected set; }
        public bool HasPreview { get => hasPreview; protected set => hasPreview = value; }
        #endregion

        public virtual void Preview(bool previewActive, bool abilityCanceled, NetworkPlayer nPlayer = null) { }
        public abstract void Tick(NetworkPlayer nPlayer);
        public abstract void Use(NetworkPlayer nPlayer, bool castOnSelf);

        protected void OnCooldown()
        {
            MessageHub.SendMessage(MessageType.AbilityCooldown, name, cooldown.Value);
            cooldown.Reduce();
        }
    }
}