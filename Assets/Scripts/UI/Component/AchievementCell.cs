using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using TMPro;

namespace UI.Component
{
    public class AchievementCell : Base<AchievementCell.Data>
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public List<Achievement> AchievementDataList = null;
        }

        [SerializeField]
        private TextMeshProUGUI titleTMP = null;
        [SerializeField]
        private Image progressImg = null;
        [SerializeField]
        private TextMeshProUGUI progressTMP = null;
        [SerializeField]
        private OpenConditionVertical openCondition = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetTitleTMP();
            SetOpenCondition();
        }

        public override void Activate()
        {
            base.Activate();

            SetProgress();
        }

        private void SetTitleTMP()
        {
            if (_data == null)
                return;

            //LocalizationSettings.StringDatabase.GetLocalizedString("Acquire", _data.TitleLocalKey, LocalizationSettings.SelectedLocale);

            titleTMP?.SetText(_data.Id.ToString());
        }

        private void SetProgress()
        {
            var achievementInfo = MainGameManager.Instance?.AcquireMgr?.GetAchievement(_data.Id);
            int step = achievementInfo != null ? achievementInfo.Step : 1;

            var achievementDatas = _data?.AchievementDataList?.OrderBy(data => data.Step);
            var achievementData = achievementDatas != null && achievementDatas.Count() >= step ? achievementDatas.ToArray()[step - 1] : null;
            float value = achievementData != null ? achievementData.Value : 0;

            float infoProgress = achievementInfo != null ? achievementInfo.Progress : 0;
            float resValue = infoProgress > value ? value : infoProgress;

            progressImg.fillAmount = resValue / value;
            progressTMP?.SetText(resValue + " / " + value);
        }

        private void SetOpenCondition()
        {
            if (openCondition == null)
                return;

            //var dailyMissionData = _data?.DailyMissionData;

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencyCashSprite(),
                Text = "x5",
            };

            openCondition.Initialize(openConditionData);
        }
    }
}

