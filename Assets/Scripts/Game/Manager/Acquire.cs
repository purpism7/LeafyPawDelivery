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

        private readonly string KeyDailyMissionDate = "KeyDailyMissionDate";
        private readonly string KeyResetDailyMission = "KeyResetDailyMission";
        private readonly string KeyGetRewardedDailyMission = "KeyGetRewardedDailyMission_{0}";
        private readonly string KeyGetRewardedAchievement = "KeyGetRewardedAchievement_{0}";

        private Info.AcquireHolder _acquireHolder = new();

        public System.DateTime? DailyMissionDateTime { get; private set; } = null;

        public override MonoBehaviour Initialize()
        {
            //DailyMissionDateTime = System.DateTime.Today.ToLocalTime().AddDays(-1);
            //PlayerPrefs.SetString(KeyDailyMissionDate, DailyMissionDateTime.Value.ToString());

            return this;
        }

        public override IEnumerator CoInitialize(Data data)
        {
            _acquireHolder?.LoadInfo();

            InitializeDailyMissionDate();

            yield break;
        }

        public void Add(Type.EAcquire eAcquire, Type.EAcquireAction eAcquireAction, int value)
        {
            _acquireHolder?.Add(eAcquire, eAcquireAction, value);

            if (CheckDailyMissionNotification)
            {
                Info.Connector.Get?.SetCompleteDailyMission(true);
            }

            if(CheckAchievementNotification)
            {
                Info.Connector.Get?.SetCompleteAchievement(true);
            }

            if(DailyMissionDateTime != null &&
               CheckResetDailyMissionDate)
            {
                ResetDailyMission();
                PlayerPrefs.SetString(KeyResetDailyMission, true.ToString());
            }
        }

        #region Daily Mission
        public Info.Acquire.DailyMission GetDailyMission(int id)
        {
            return _acquireHolder?.GetDailyMission(id);
        } 

        private void InitializeDailyMissionDate()
        {
            string dailyMissionDate = PlayerPrefs.GetString(KeyDailyMissionDate);
            if (string.IsNullOrEmpty(dailyMissionDate))
            {
                ResetDailyMission();
                PlayerPrefs.SetString(KeyResetDailyMission, true.ToString());

                return;
            }

            DailyMissionDateTime = System.DateTime.Parse(dailyMissionDate);
            if (CheckResetDailyMissionDate)
            {
                ResetDailyMission();
                PlayerPrefs.SetString(KeyResetDailyMission, true.ToString());
            }
        }

        private void SetDailyMissionDate()
        {
            DailyMissionDateTime = System.DateTime.Today.ToLocalTime().AddDays(1);
            PlayerPrefs.SetString(KeyDailyMissionDate, DailyMissionDateTime.Value.ToString());
        }

        public bool CheckResetDailyMissionDate
        {
            get
            {
                if (DailyMissionDateTime == null)
                    return true;

                return (DailyMissionDateTime.Value - System.DateTime.UtcNow.ToLocalTime()).TotalSeconds < 0;
            }
        }

        public bool CheckResetDailyMission
        {
            get
            {
                if(System.Boolean.TryParse(PlayerPrefs.GetString(KeyResetDailyMission), out bool reset))
                {
                    if(reset)
                    {
                        PlayerPrefs.SetString(KeyResetDailyMission, false.ToString());
                    }

                    return reset;
                }

                return false;
            }
        }

        public void ResetDailyMission()
        {
            Info.Connector.Get?.SetCompleteDailyMission(false);

            _acquireHolder?.ResetDailyMission();

            SetDailyMissionDate();
        }

        public bool GetRewardDailyMission(int id)
        {
            System.Boolean.TryParse(PlayerPrefs.GetString(string.Format(KeyGetRewardedDailyMission, id), false.ToString()), out bool getRewarded);

            return getRewarded;
        }

        public void SetGetRewardDailyMission(int id, bool getReward)
        {
            PlayerPrefs.SetString(string.Format(KeyGetRewardedDailyMission, id), getReward.ToString());
        }

        public bool CheckDailyMissionNotification
        {
            get
            {
                var dailyMissions = DailyMissionContainer.Instance?.Datas;
                if (dailyMissions == null)
                    return false;

                foreach (var dailyMission in dailyMissions)
                {
                    if (dailyMission == null)
                        continue;

                    var info = GetDailyMission(dailyMission.Id);
                    if (info == null)
                        continue;

                    if (GetRewardDailyMission(dailyMission.Id))
                        continue;

                    if (info.Progress >= dailyMission.Value)
                        return true;
                }

                return false;
            }
        }
        #endregion

        #region Achievement
        public Info.Acquire.Achievement GetAchievement(int id)
        {
            return _acquireHolder?.GetAchievement(id);
        }

        public void SetNextStep(int id)
        {
            _acquireHolder?.SetNextStep(id);
        }

        public bool GetRewardAchievement(int id)
        {
            bool getRewarded = true;
            System.Boolean.TryParse(PlayerPrefs.GetString(string.Format(KeyGetRewardedAchievement, id), false.ToString()), out getRewarded);

            return getRewarded;
        }

        public void SetGetRewardAhchievement(int id, bool getReward)
        {
            PlayerPrefs.SetString(string.Format(KeyGetRewardedAchievement, id), getReward.ToString());
        }

        public bool CheckAchievementNotification
        {
            get
            {
                var achievements = AchievementContainer.Instance?.Datas;
                if (achievements == null)
                    return false;

                foreach (var achievement in achievements)
                {
                    if (achievement == null)
                        continue;

                    var info = GetAchievement(achievement.Id);
                    if (info == null)
                        continue;

                    if (info.Step != achievement.Step)
                        continue;

                    if (GetRewardAchievement(achievement.Id))
                        continue;

                    if (info.Progress >= achievement.Value)
                        return true;
                }

                return false;
            }
        }
        #endregion
    }
}

 