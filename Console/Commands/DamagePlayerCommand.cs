using Photon.Pun;
using System;
using WatStudios.DeepestDungeon.Core.PlayerLogic;

namespace WatStudios.DeepestDungeon.Tools.Console
{
    internal class DamagePlayerCommand : ConsoleCommand
    {
        internal DamagePlayerCommand()
        {
            Name = "Damage";
            Command = "Damage";
            Description = "Damages the specified player";
            Help = $"<color=red>{Command}</color> <playername> <amount>";
            SuccessReturnText = string.Empty;
            FailureReturnText = "Damaging Failed: Only the master client can damage other players.";
            TakesArguments = true;
        }

        #region Public Methods
        internal override string RunCommand(string[] args)
        {
            NetworkPlayer networkPlayer = Array.Find(UnityEngine.Object.FindObjectsOfType<NetworkPlayer>(), player => player.photonView.Owner.NickName == args[0]);
            int.TryParse(args[1], out int amount);
            if (networkPlayer == null)
                return $"Damaging Failed: There is no player with the name {args[0]} in the room.";
            if(amount <= 0)
                return $"Damaging Failed: Amount must be greater than zero.";

            if (NetworkPlayer.LocalPlayerInstance.photonView.Owner.IsMasterClient ||
                networkPlayer.photonView.Owner.ActorNumber == NetworkPlayer.LocalPlayerInstance.photonView.OwnerActorNr)
            {
                networkPlayer.photonView.RPC("TakeDamage", RpcTarget.All, amount, NetworkPlayer.LocalPlayerInstance.photonView.ViewID);
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
