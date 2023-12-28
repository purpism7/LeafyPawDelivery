using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using System;

using TMPro;


namespace UI.Component
{
    public interface IDailyMission
    {
        bool IsCompleted { get; }
        bool GetRewarded { get; }
        void Reset();
    }

    public class DailyMissionCell : Base<DailyMissionCell.Data>, IDailyMission
    {
        public class Data : BaseData
        {
            public DailyMission DailyMissionData = null;
            public bool isTotal = false;
        }

        private const string KeyGetRewardedDailyMission = "KeyGetRewardedDailyMission_{0}";

        [SerializeField]
        private TextMeshProUGUI titleTMP = null;
        [SerializeField]
        private Image progressImg = null;
        [SerializeField]
        private TextMeshProUGUI progressTMP = null;
        //[SerializeField]
        //private Button getRewardBtn = null;
        [SerializeField]
        private OpenConditionVertical openCondition = null;
        [SerializeField]
        private RectTransform completedRootRectTm = null;

        private float _progress = 0;

        public int Id { get { return _data?.DailyMissionData != null ? _data.DailyMissionData.Id : 0; } }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetOpenCondition();
        }

        public override void Activate()
        {
            base.Activate();

            SetTitleTMP();
            SetProgress();

            //if(getRewardBtn != null)
            //{
            //    getRewardBtn.interactable = _progress < DataProgress;
            //}

            UIUtils.SetActive(completedRootRectTm, GetRewarded);
        }

        private void SetTitleTMP()
        {
            if (_data == null)
                return;

            string localKey = string.Empty;
            string title = string.Empty;
            if(_data.isTotal)
            {
                localKey = "dailymission_total";
                title = LocalizationSettings.StringDatabase.GetLocalizedString("Acquire", localKey, LocalizationSettings.SelectedLocale);
            }
            else
            {
                var dailyMissionData = _data?.DailyMissionData;
                if (dailyMissionData == null)
                    return;

                localKey = "dailymission_" + dailyMissionData.Id;
                title = string.Format(LocalizationSettings.StringDatabase.GetLocalizedString("Acquire", localKey, LocalizationSettings.SelectedLocale), dailyMissionData.Value);
            }
           
            titleTMP?.SetText(title);
        }

        private void SetProgress()
        {
            if (_data == null)
                return;

            if(_data.isTotal)
            {

            }

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
                Text = "5",
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

        private bool GetRewarded
        {
            get
            {
                bool getRewarded = true;
                var dailyMissionData = _data?.DailyMissionData;
                if (dailyMissionData == null)
                    return getRewarded;

                if(Boolean.TryParse(PlayerPrefs.GetString(string.Format(KeyGetRewardedDailyMission, _data.DailyMissionData.Id), false.ToString()), out getRewarded))
                {
                    return getRewarded;
                }

                return getRewarded;
            }
        }

        private bool IsCompleted
        {
            get
            {
                var dailyMissionData = _data?.DailyMissionData;
                if (dailyMissionData == null)
                    return false;

                return _progress >= dailyMissionData.Progress;
            }
        }

        #region IDailyMission
        bool IDailyMission.IsCompleted
        {
            get
            {
                return IsCompleted;
            }
        }

        bool IDailyMission.GetRewarded
        {
            get
            {
                return GetRewarded;
            }
        }

        void IDailyMission.Reset()
        {
            var dailyMissionData = _data?.DailyMissionData;
            if (dailyMissionData == null)
                return;

            PlayerPrefs.SetString(string.Format(KeyGetRewardedDailyMission, dailyMissionData.Id), false.ToString());
        }
        #endregion

        public void OnClickGetReward()
        {
            var dataProgress = DataProgress;
            if (dataProgress <= 0)
                return;

            if (_progress < dataProgress)
                return;

            Game.UIManager.Instance?.Top?.CollectCashCurrency(openCondition.transform.position, 5);

            UIUtils.SetActive(completedRootRectTm, true);

            if(transform)
            {
                transform.SetAsLastSibling();
            }

            PlayerPrefs.SetString(string.Format(KeyGetRewardedDailyMission, _data.DailyMissionData.Id), true.ToString());
        }
    }
}

