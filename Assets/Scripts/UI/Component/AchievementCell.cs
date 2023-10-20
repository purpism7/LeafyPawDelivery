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
        private Button getRewardBtn = null;
        [SerializeField]
        private OpenConditionVertical openCondition = null;

        private int _step = 1;
        private float _progress = 0;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            SetOpenCondition();
            SetTitleTMP();
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

            titleTMP?.SetText(_data.Id.ToString() + " - " + _step);
        }

        private void SetProgress()
        {
            int id = _data != null ? _data.Id : 0;

            var achievementInfo = MainGameManager.Instance?.AcquireMgr?.GetAchievement(id);
            _step = achievementInfo != null ? achievementInfo.Step : 1;

            float dataProgress = DataProgress;
            float infoProgress = achievementInfo != null ? achievementInfo.Progress : 0;
            _progress = infoProgress > dataProgress ? dataProgress : infoProgress;

            progressImg.fillAmount = _progress / dataProgress;
            progressTMP?.SetText(_progress + " / " + dataProgress);
        }

        private float DataProgress
        {
            get
            {
                var achievementDatas = _data?.AchievementDataList?.OrderBy(data => data.Step);
                var achievementData = achievementDatas != null && achievementDatas.Count() >= _step ? achievementDatas.ToArray()[_step - 1] : null;

                return achievementData != null ? achievementData.Value : 0;
            }
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

        public void OnClickGetReward()
        {

        }
    }
}

