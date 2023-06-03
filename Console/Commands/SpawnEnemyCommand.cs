using UnityEngine;
using WatStudios.DeepestDungeon.Core.EnemyLogic;
using WatStudios.DeepestDungeon.Messaging;
using NetworkPlayer = WatStudios.DeepestDungeon.Core.PlayerLogic.NetworkPlayer;

namespace WatStudios.DeepestDungeon.Tools.Console
{
    internal class SpawnEnemyCommand : ConsoleCommand
    {
        internal SpawnEnemyCommand()
        {
            Name = "Spawn Enemy";
            Command = "spawn";
            Description = "Spawns the specified enemy near you or at given coordinates.";
            Help = $"<color=red>{Command}</color> <enemyName> <x> <y> <z>";
            SuccessReturnText = "Success: {0} was spawned at {1}.";
            FailureReturnText = "Failure: Only the master client can spawn enemies.";
            TakesArguments = true;
        }

        internal override string RunCommand(string[] args)
        {
            if (!NetworkPlayer.LocalPlayerInstance.photonView.Owner.IsMasterClient)
                return FailureReturnText;

            string enemyName = args[0];
            Vector3 spawnPosition = Vector3.zero;
            if (args.Length > 1)
            {
                int[] spawnPosArr = new int[3];
                for (int i = 1; i < args.Length; i++)
                {
                    if (!int.TryParse(args[i], out spawnPosArr[i - 1]))
                        return $"Failure: {args[i]} is not a number!";
                }
                spawnPosition = new Vector3(spawnPosArr[0], spawnPosArr[1], spawnPosArr[2]);
            }

            try
            {
                if (EnemyManager.Instance.SpawnEnemy(spawnPosition, enemyName))
                {
                    SuccessReturnText = SuccessReturnText.Replace("{0}", enemyName);
                    SuccessReturnText = SuccessReturnText.Replace("{1}", spawnPosition.ToString());
                    return SuccessReturnText;
                }
                else
                {
                    return $"Failure: No enemy with the name {enemyName} found!";
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return "Failure: EnemyManager is null or not activated!";
            }
        }
    }
}