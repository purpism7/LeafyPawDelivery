using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace UI.Component
{
    public class AchievementCell : Base<AchievementCell.Data>
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public List<Achievement> AchievementList = null;
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

        private void SetTitleTMP()
        {
            if (_data == null)
                return;

            //LocalizationSettings.StringDatabase.GetLocalizedString("Acquire", _data.TitleLocalKey, LocalizationSettings.SelectedLocale);

            titleTMP?.SetText(_data.Id.ToString());
        }
    }
}

