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

        public Acquire Acquire { get; private set; } = new();

        public override void LoadInfo()
        {
            if (!System.IO.Directory.Exists(JsonFilePath))
            {
                System.IO.Directory.CreateDirectory(JsonFilePath);
            }

            LoadDailyMissionInfo();
        }

        private void LoadDailyMissionInfo()
        {
            var fullPath = JsonFilePath + DailyMissionJsonFileName;
            if (!System.IO.File.Exists(fullPath))
                return;

            var jsonString = System.IO.File.ReadAllText(fullPath);
            var dailyMissionInfos  = JsonHelper.FromJson<Acquire.DailyMission>(jsonString);

            Acquire?.DailyMissionInfoList?.AddRange(dailyMissionInfos);
        }

        private void SaveInfo<T>(string jsonFileName, List<T> list)
        {
            var jsonString = JsonHelper.ToJson(list.ToArray());
            Debug.Log("jsonString = " + jsonString);

            System.IO.File.WriteAllText(JsonFilePath + jsonFileName, jsonString);
        }

        private void SaveInfo()
        {
            var jsonString = JsonUtility.ToJson(Acquire);
            Debug.Log("jsonString = " + jsonString);

            System.IO.File.WriteAllText(JsonFilePath + "Acquire.json", jsonString);
        }

        private void SaveDailyMissionInfo()
        {
            SaveInfo(DailyMissionJsonFileName, Acquire.DailyMissionInfoList);
        }

        private void SaveAchievementInfo()
        {

        }

        public void Add(Game.Type.EAcquire eAcquire, Game.Type.EAcquireAction eAcquireAction, int value)
        {
            if (Acquire == null)
                return;

            Acquire.AddDailyMission(eAcquire, eAcquireAction, value);

            //SaveInfo();
            SaveDailyMissionInfo();
        }
    }
}

