using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Localization.Settings;

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
        }

        public override void Activate()
        {
            base.Activate();

            SetStep();
            SetTitleTMP();
            SetProgress();
        }

        private void SetStep()
        {
            int id = _data != null ? _data.Id : 0;
            var achievementInfo = MainGameManager.Instance?.AcquireMgr?.GetAchievement(id);

            _step = achievementInfo != null ? achievementInfo.Step : 1;
        }

        private void SetTitleTMP()
        {
            if (_data == null)
                return;

            string localKey = "achievement_" + _data.Id;
            var title = LocalizationSettings.StringDatabase.GetLocalizedString("Acquire", localKey, LocalizationSettings.SelectedLocale);
            title = string.Format(title, DataProgress);

            titleTMP?.SetText(title);
        }

        private void SetProgress()
        {
            int id = _data != null ? _data.Id : 0;
            var achievementInfo = MainGameManager.Instance?.AcquireMgr?.GetAchievement(_data.Id);

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
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                Text = "x5",
            };

            openCondition.Initialize(openConditionData);
        }

        public void OnClickGetReward()
        {
            var dataProgress = DataProgress;
            if (dataProgress <= 0)
                return;

            if (_progress < dataProgress)
                return;

            Game.UIManager.Instance?.Top?.CollectCashCurrency(openCondition.transform.position, 5);

            if(_data != null)
            {
                MainGameManager.Instance?.AcquireMgr?.SetNextStep(_data.Id);
            }

            SetStep();
            SetTitleTMP();
            SetProgress();
        }
    }
}

