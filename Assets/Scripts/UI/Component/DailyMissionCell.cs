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

            var record = MainGameManager.Instance?.RecordContainer?.Get(dailyMissionData.EAcquireType, dailyMissionData.EAcquireActionType);
            float recordValue = record != null ? record.Value : 0;
            float resValue = recordValue > value ? value : recordValue;

            progressImg.fillAmount = resValue / value;
            progressTMP?.SetText(resValue + " / " + value);
        }
    }
}

