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
            public string TitleLocalKey = string.Empty;
            public int RewardId = 0;
        }

        [SerializeField]
        private TextMeshProUGUI titleTMP = null;
        [SerializeField]
        private Image progressImg = null;
        [SerializeField]
        private TextMeshProUGUI progressTMP = null;

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

            LocalizationSettings.StringDatabase.GetLocalizedString("Acquire", _data.TitleLocalKey, LocalizationSettings.SelectedLocale);

            titleTMP?.SetText(_data.Id.ToString());
        }
    }
}

