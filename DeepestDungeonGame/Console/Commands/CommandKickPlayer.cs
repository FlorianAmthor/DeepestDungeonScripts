using Photon.Pun;
using System;
using WatStudios.DeepestDungeon.Core.PlayerLogic;

namespace WatStudios.DeepestDungeon.Tools.Console
{
    internal class CommandKickPlayer : ConsoleCommand
    {
        internal CommandKickPlayer()
        {
            Name = "Kick Player";
            Command = "kick";
            Description = "Kicks the specified player";
            Help = $"<color=red>{Command}</color> <playername>";
            SuccessReturnText = "Success: {0} was kicked from the room.";
            FailureReturnText = "Kicking Failed: Only the master client can kick other players.";
            TakesArguments = true;
        }

        #region Public Methods
        internal override string RunCommand(string[] args)
        {
            var networkPlayer = Array.Find(UnityEngine.Object.FindObjectsOfType<NetworkPlayer>(), player => player.photonView.Owner.NickName == args[0]);

            if (networkPlayer == null)
                return $"Killing Failed: There is no player with the name {args[0]} in the room.";

            if (networkPlayer.photonView.Owner.ActorNumber == NetworkPlayer.LocalPlayerInstance.photonView.OwnerActorNr)
                return "Kicking Failed: You can't kick yourself";

            if (NetworkPlayer.LocalPlayerInstance.photonView.Owner.IsMasterClient)
            {
                PhotonNetwork.CloseConnection(networkPlayer.photonView.Owner);
                SuccessReturnText.Replace("{0}", networkPlayer.photonView.Owner.NickName);
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
