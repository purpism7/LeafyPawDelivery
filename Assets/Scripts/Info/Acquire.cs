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

        #region DailyMission
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

        public DailyMission GetDailyMission(int id)
        {
            if (DailyMissionInfoList == null)
                return null;

            return DailyMissionInfoList.Find(dailyMission => dailyMission.Id == id);
        }
        #endregion

        #region Achievement
        public void AddAchievement(Game.Type.EAcquire eAcquire, Game.Type.EAcquireAction eAcquireAction, int value)
        {
            var achievementData = AchievementContainer.Instance?.GetData(1, eAcquire, eAcquireAction);
            if (achievementData == null)
                return;

            Achievement achievementInfo = null;
            foreach (var info in AchievementInfoList)
            {
                if (info == null)
                    continue;

                if (info.Id != achievementData.Id)
                    continue;

                achievementInfo = info;
                info.Progress += value;

                if (achievementData.Value <= info.Progress)
                {
                    info.Progress = achievementData.Value;
                }

                break;
            }

            if (achievementInfo == null)
            {
                AchievementInfoList.Add(
                    new Achievement()
                    {
                        Id = achievementData.Id,
                        Step = 1,
                        Progress = value,
                    });
            }
        }

        public void SetNextStep(int id)
        {
            if (AchievementInfoList == null)
                return;

            foreach(var achievementInfo in AchievementInfoList)
            {
                if (achievementInfo == null)
                    continue;

                if (achievementInfo.Id != id)
                    continue;

                var achievementData = AchievementContainer.Instance.GetData(id, achievementInfo.Step);
                if (achievementData == null)
                    continue;

                if (achievementInfo.Progress < achievementData.Value)
                    continue;

                achievementData = AchievementContainer.Instance.GetData(id, achievementInfo.Step + 1);
                if (achievementData != null)
                {
                    achievementInfo.Step = achievementData.Step;

                    break;
                }
            }
        }

        public Achievement GetAchievement(int id)
        {
            if (AchievementInfoList == null)
                return null;

            return AchievementInfoList.Find(achievement => achievement.Id == id);
        }
        #endregion
    }
}

