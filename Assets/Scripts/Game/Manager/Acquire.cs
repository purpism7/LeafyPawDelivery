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
        private System.DateTime? _dailyMissionDateTime = null;
        
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
            Debug.Log(dailyMissionDate);
            if (string.IsNullOrEmpty(dailyMissionDate))
            {
                _dailyMissionDateTime = System.DateTime.UtcNow;
                PlayerPrefs.SetString(KeyDailyMissionDate, _dailyMissionDateTime.Value.ToString());

                return;
            }

            if(CheckResetDailyMission)
            {
                _dailyMissionDateTime = System.DateTime.UtcNow;
                PlayerPrefs.SetString(KeyDailyMissionDate, _dailyMissionDateTime.Value.ToString());

                return;
            }

            _dailyMissionDateTime = System.DateTime.Parse(dailyMissionDate);
        }

        public bool CheckResetDailyMission
        {
            get
            {
                if (_dailyMissionDateTime == null)
                    return false;

                return (System.DateTime.Parse(_dailyMissionDateTime.Value.ToString()).AddDays(1) - System.DateTime.UtcNow).TotalDays >= 1;
            }
        }

        public void ResetDailyMission()
        {
            _acquireHolder?.ResetDailyMission();

            SetDailyMissionDate();
        }
    }
}

 