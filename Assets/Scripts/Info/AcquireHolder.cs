using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Info
{
    public class AcquireHolder : Holder.Base
    {
        protected override string JsonFilePath => RootJsonFilePath;

        private const string _dailyMissionJsonFileName = "DailyMission.txt";
        private const string _achievementJsonFileName = "Achievement.txt";
        private const string _secretKey = "hANkyUlAcquire";

        private Acquire _acquire = new();

        public override void LoadInfo()
        {
            RootJsonFilePath = Utility.GetInfoPath();

            if (!System.IO.Directory.Exists(RootJsonFilePath))
            {
                System.IO.Directory.CreateDirectory(RootJsonFilePath);
            }

            LoadDailyMissionInfo();
            LoadAchievementInfo();
        }

        private void LoadDailyMissionInfo()
        {
            var fullPath = Path.Combine(JsonFilePath, _dailyMissionJsonFileName);
            if (!System.IO.File.Exists(fullPath))
                return;

            var encryptStr = System.IO.File.ReadAllText(fullPath);
            var jsonStr = encryptStr.Decrypt(_secretKey);
            var dailyMissionInfos  = JsonHelper.FromJson<Acquire.DailyMission>(jsonStr);

            _acquire?.dailyMissionInfoList?.AddRange(dailyMissionInfos);
        }

        private void LoadAchievementInfo()
        {
            var fullPath = Path.Combine(JsonFilePath, _achievementJsonFileName);
            if (!System.IO.File.Exists(fullPath))
                return;

            var encryptStr = System.IO.File.ReadAllText(fullPath);
            var jsonStr = encryptStr.Decrypt(_secretKey);
            var achievementInfos = JsonHelper.FromJson<Acquire.Achievement>(jsonStr);

            _acquire?.AchievementInfoList?.AddRange(achievementInfos);
        }

        private void SaveInfo<T>(string jsonFileName, List<T> list)
        {
            var jsonStr = JsonHelper.ToJson(list.ToArray());
            var encryptStr = jsonStr.Encrypt(_secretKey);
            var fullPath = Path.Combine(JsonFilePath, jsonFileName);

            System.IO.File.WriteAllText(fullPath, encryptStr);
        }

        private void SaveDailyMissionInfo()
        {
            SaveInfo(_dailyMissionJsonFileName, _acquire.dailyMissionInfoList);
        }

        private void SaveAchievementInfo()
        {
            SaveInfo(_achievementJsonFileName, _acquire.AchievementInfoList);
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

