using Photon.Pun;

namespace WatStudios.DeepestDungeon.Core.AbilitySystem.StatusEffects
{
    public interface IStatusEntity
    {
        StatusEffectHandler StatusEffectHandler { get; }
        PhotonView PhotonView { get; }
    }
}