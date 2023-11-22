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
        private Button getRewardBtn = null;
        [SerializeField]
        private OpenConditionVertical openCondition = null;
        [SerializeField]
        private RectTransform completedRootRectTm = null;

        private float _progress = 0;

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

            //if(getRewardBtn != null)
            //{
            //    getRewardBtn.interactable = _progress < DataProgress;
            //}

            UIUtils.SetActive(completedRootRectTm, _progress >= DataProgress);
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

            float dataProgress = DataProgress;
            int id = dailyMissionData != null ? dailyMissionData.Id : 0;

            var dailyMissionInfo = MainGameManager.Get<Game.Manager.Acquire>()?.GetDailyMission(id);

            float infoProgress = dailyMissionInfo != null ? dailyMissionInfo.Progress : 0;
            _progress = infoProgress > dataProgress ? dataProgress : infoProgress;

            progressImg.fillAmount = _progress / dataProgress;
            progressTMP?.SetText(_progress + " / " + dataProgress);
        }

        private void SetOpenCondition()
        {
            if (openCondition == null)
                return;

            var dailyMissionData = _data?.DailyMissionData;

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                Text = "x5",
            };

            openCondition.Initialize(openConditionData);
        }

        private float DataProgress
        {
            get
            {
                var dailyMissionData = _data?.DailyMissionData;

                return dailyMissionData != null ? dailyMissionData.Value : 0;
            }
        }

        public void OnClickGetReward()
        {
            var dataProgress = DataProgress;
            if (dataProgress <= 0)
                return;

            if (_progress < dataProgress)
                return;

            Game.UIManager.Instance?.Top?.CollectCashCurrency(openCondition.transform.position, 5);

            UIUtils.SetActive(completedRootRectTm, true);
        }
    }
}

