using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class Acquire
    {
        [System.Serializable]
        public class Achievement
        {
            public int Id = 0;
            public int Step = 0;
            public int Progress = 0;
        }

        [System.Serializable]
        public class DailyMission
        {
            public int Id = 0;
            public int Progress = 0;
        }

        public List<Achievement> AchievementList = new();
        public List<DailyMission> DailyMissionList = new(); 
    }
}

