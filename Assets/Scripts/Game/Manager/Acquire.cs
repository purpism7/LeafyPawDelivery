using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class Acquire : Base<Acquire.Data>
    {
        public class Data : BaseData
        {

        }

        private const string KeyDailyMissionDate = "KeyDailyMissionDate";

        private Info.AcquireHolder _acquireHolder = new();
        
        protected override void Initialize()
        {
            SetDailyMissionDate();
        }

        public override IEnumerator CoInitialize(Data data)
        {
            _acquireHolder?.LoadInfo();

            yield break;
        }

        public void Add(Type.EAcquire eAcquire, Type.EAcquireAction eAcquireAction, int value)
        {
            _acquireHolder?.Add(eAcquire, eAcquireAction, value);
        }

        public void SetNextStep(int id)
        {
            _acquireHolder?.SetNextStep(id);
        }

        public Info.Acquire.DailyMission GetDailyMission(int id)
        {
            return _acquireHolder?.GetDailyMission(id);
        }

        public Info.Acquire.Achievement GetAchievement(int id)
        {
            return _acquireHolder?.GetAchievement(id);
        }

        private void SetDailyMissionDate()
        {
            string dailyMissionDate = PlayerPrefs.GetString(KeyDailyMissionDate);
            if (string.IsNullOrEmpty(dailyMissionDate))
            {
                PlayerPrefs.SetString(KeyDailyMissionDate, System.DateTime.UtcNow.ToString());

                return;
            }

            if(CheckResetDailyMission)
            {
                PlayerPrefs.SetString(KeyDailyMissionDate, System.DateTime.UtcNow.ToString());
            }
        }

        public bool CheckResetDailyMission
        {
            get
            {
                return (System.DateTime.UtcNow - System.DateTime.Parse(PlayerPrefs.GetString(KeyDailyMissionDate))).Days >= 1;
            }
        }

        public void ResetDailyMission()
        {
            _acquireHolder?.ResetDailyMission();

            SetDailyMissionDate();
        }
    }
}

 