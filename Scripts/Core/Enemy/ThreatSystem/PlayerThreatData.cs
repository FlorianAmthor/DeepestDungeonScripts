using System;

namespace WatStudios.DeepestDungeon.Core.EnemyLogic.ThreatSystem
{
    /// <summary>
    /// Data Class for Threatmanagement
    /// </summary>
    internal struct PlayerThreatData : IComparable<PlayerThreatData>
    {
        internal PlayerThreatData(int threat)
        {
            Priority = Priority.Normal;
            Threat = threat;
            DmgInFrame = 0;
            HealInFrame = 0;
        }
        internal PlayerThreatData(Priority priority, int threat)
        {
            Priority = priority;
            Threat = threat;
            DmgInFrame = 0;
            HealInFrame = 0;
        }

        internal Priority Priority;
        internal int Threat;
        internal float DmgInFrame;
        internal float HealInFrame;

        internal void Reset()
        {
            Priority = Priority.Normal;
            Threat = 0;
            DmgInFrame = 0;
            HealInFrame = 0;
        }

        public int CompareTo(PlayerThreatData other)
        {
            var result = Priority.CompareTo(other.Priority);
            if (result == 0)
            {
                result = Threat.CompareTo(other.Threat);
            }
            return result;
        }
    }
}
