using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

using TMPro;

namespace UI.Component
{
    public class DailyMissionCell : Base<DailyMissionCell.Data>
    {
        public class Data : BaseData
        {
            public DailyMission DailyMissionData = null;
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

            Debug.Log("DailyMissionCell Activate");
            SetProgress();
        }

        private void SetTitleTMP()
        {
            var dailyMissionData = _data?.DailyMissionData;
            if (dailyMissionData == null)
                return;

            string localKey = "dailymission_" + dailyMissionData.Id;
            var title = string.Format(LocalizationSettings.StringDatabase.GetLocalizedString("Acquire", localKey, LocalizationSettings.SelectedLocale), dailyMissionData.Value);

            titleTMP?.SetText(title);
        }

        private void SetProgress()
        {
            var dailyMissionData = _data?.DailyMissionData;
            float value = dailyMissionData != null ? dailyMissionData.Value : 0;

            var dailyMissionInfo = MainGameManager.Instance?.AcquireMgr?.GetDailyMission(dailyMissionData.Id);

            float infoProgress = dailyMissionInfo != null ? dailyMissionInfo.Progress : 0;
            float resValue = infoProgress > value ? value : infoProgress;

            progressImg.fillAmount = resValue / value;
            progressTMP?.SetText(resValue + " / " + value);
        }

        private void SetOpenCondition()
        {
            if (openCondition == null)
                return;

            var dailyMissionData = _data?.DailyMissionData;

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencyCashSprite(),
                Text = "x5",
            };

            openCondition.Initialize(openConditionData);
        }
    }
}

