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

        public List<Achievement> AchievementInfoList = new();
        public List<DailyMission> DailyMissionInfoList = new();

        public void AddDailyMission(Game.Type.EAcquire eAcquire, Game.Type.EAcquireAction eAcquireAction, int value)
        {
            var dailyMissionData = DailyMissionContainer.Instance?.GetData(eAcquire, eAcquireAction);
            if (dailyMissionData == null)
                return;

            DailyMission dailyMissionInfo = null;
            foreach (var info in DailyMissionInfoList)
            {
                if (info == null)
                    continue;

                if(info.Id != dailyMissionData.Id)
                    continue;

                dailyMissionInfo = info;
                info.Progress += value;

                if (dailyMissionData.Value <= info.Progress)
                {
                    info.Progress = dailyMissionData.Value;
                }

                break;
            }

            if(dailyMissionInfo ==null)
            {
                DailyMissionInfoList.Add(
                    new DailyMission()
                    {
                        Id = dailyMissionData.Id,
                        Progress = value,
                    });
            }
            
        }
    }
}

