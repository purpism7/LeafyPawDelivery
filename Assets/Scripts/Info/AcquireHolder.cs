using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class AcquireHolder : Holder.Base
    {
        protected override string JsonFilePath => RootJsonFilePath + "/Info/";
        private readonly string DailyMissionJsonFileName = "DailyMission.json";
        readonly private string AchievementJsonFileName = "Achievement.json";

        private Acquire _acquire = new();

        public override void LoadInfo()
        {
            if (!System.IO.Directory.Exists(JsonFilePath))
            {
                System.IO.Directory.CreateDirectory(JsonFilePath);
            }

            LoadDailyMissionInfo();
            LoadAchievementInfo();
        }

        private void LoadDailyMissionInfo()
        {
            var fullPath = JsonFilePath + DailyMissionJsonFileName;
            if (!System.IO.File.Exists(fullPath))
                return;

            var jsonString = System.IO.File.ReadAllText(fullPath);
            var dailyMissionInfos  = JsonHelper.FromJson<Acquire.DailyMission>(jsonString);

            _acquire?.dailyMissionInfoList?.AddRange(dailyMissionInfos);
        }

        private void LoadAchievementInfo()
        {
            var fullPath = JsonFilePath + AchievementJsonFileName;
            if (!System.IO.File.Exists(fullPath))
                return;

            var jsonString = System.IO.File.ReadAllText(fullPath);
            var achievementInfos = JsonHelper.FromJson<Acquire.Achievement>(jsonString);

            _acquire?.AchievementInfoList?.AddRange(achievementInfos);
        }

        private void SaveInfo<T>(string jsonFileName, List<T> list)
        {
            var jsonString = JsonHelper.ToJson(list.ToArray());
            Debug.Log("jsonString = " + jsonString);

            System.IO.File.WriteAllText(JsonFilePath + jsonFileName, jsonString);
        }

        private void SaveDailyMissionInfo()
        {
            SaveInfo(DailyMissionJsonFileName, _acquire.dailyMissionInfoList);
        }

        private void SaveAchievementInfo()
        {
            SaveInfo(AchievementJsonFileName, _acquire.AchievementInfoList);
        }

        public void Add(Game.Type.EAcquire eAcquire, Game.Type.EAcquireAction eAcquireAction, int value)
        {
            if (_acquire == null)
                return;

            _acquire.AddDailyMission(eAcquire, eAcquireAction, value);
            _acquire.AddAchievement(eAcquire, eAcquireAction, value);

            SaveDailyMissionInfo();
            SaveAchievementInfo();
        }

        public void SetNextStep(int id)
        {
            if (_acquire == null)
                return;

            _acquire.SetNextStep(id);

            SaveAchievementInfo();
        }

        public Acquire.DailyMission GetDailyMission(int id)
        {
            return _acquire?.GetDailyMission(id);
        }

        public Acquire.Achievement GetAchievement(int id)
        {
            return _acquire?.GetAchievement(id);
        }

        public void ResetDailyMission()
        {
            _acquire?.dailyMissionInfoList?.Clear();

            SaveDailyMissionInfo();
        }
    }
}

