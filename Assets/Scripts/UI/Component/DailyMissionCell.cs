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
            public int Id = 0;
            public int Value = 0;
            public int RewardId = 0;
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
        }

        private void SetTitleTMP()
        {
            if (_data == null)
                return;

            string localKey = "dailymission_" + _data.Id;
            var title = string.Format(LocalizationSettings.StringDatabase.GetLocalizedString("Acquire", localKey, LocalizationSettings.SelectedLocale), _data.Value);

            titleTMP?.SetText(title);
        }
    }
}

