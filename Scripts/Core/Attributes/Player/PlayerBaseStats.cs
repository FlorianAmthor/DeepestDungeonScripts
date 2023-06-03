using System.Collections.Generic;
using UnityEngine;
using WatStudios.DeepestDungeon.Core.AbilitySystem.Abilities;

namespace WatStudios.DeepestDungeon.Core.Attributes
{
    [CreateAssetMenu(fileName = "PlayerBaseStats", menuName = "ScriptableObjects/AttributeSheets/PlayerBaseStats")]
    public class PlayerBaseStats : EntityBaseStats
    {
        #region Attributes
        public ThreatMultiplier ThreatMultiplier;
        public HealEffectivity HealEffectivity;
        public ShieldPoints ShieldPoints;
        public DamageModifier DamageModifier;
        #endregion

        #region Abilities
        [Header("Abilities")]
        [SerializeField] private List<Ability> _abilities;
        public List<Ability> Abilities => _abilities;
        #endregion
    }
}
