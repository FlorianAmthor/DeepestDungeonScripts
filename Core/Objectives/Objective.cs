using UnityEngine;

namespace WatStudios.DeepestDungeon.Core.Objectives
{
    public abstract class Objective : ScriptableObject
    {
        public string objectiveName;
        public string description;
        public string missionText;
    }

    public class KillObjective : Objective
    {
        public int numOfMonstersToKill;
    }
}