using Photon.Pun;
using System;
using WatStudios.DeepestDungeon.Core.PlayerLogic;

namespace WatStudios.DeepestDungeon.Tools.Console
{
    internal class HealPlayerCommand : ConsoleCommand
    {
        internal HealPlayerCommand()
        {
            Name = "Heal";
            Command = "Heal";
            Description = "Heals the specified player";
            Help = $"<color=red>{Command}</color> <playername>";
            SuccessReturnText = string.Empty;
            FailureReturnText = "Healing Failed: Only the master client can heal other players.";
            TakesArguments = true;
        }

        #region Public Methods
        internal override string RunCommand(string[] args)
        {
            NetworkPlayer networkPlayer = Array.Find(UnityEngine.Object.FindObjectsOfType<NetworkPlayer>(), player => player.photonView.Owner.NickName == args[0]);

            if (networkPlayer == null)
                return $"Healing Failed: There is no player with the name {args[0]} in the room.";

            if (NetworkPlayer.LocalPlayerInstance.photonView.Owner.IsMasterClient ||
                networkPlayer.photonView.Owner.ActorNumber == NetworkPlayer.LocalPlayerInstance.photonView.OwnerActorNr)
            {
                networkPlayer.photonView.RPC("RecieveHealing", RpcTarget.All, 20000);
                return SuccessReturnText;
            }
            else
            {
                return FailureReturnText;
            }
        }
        #endregion
    }
}
