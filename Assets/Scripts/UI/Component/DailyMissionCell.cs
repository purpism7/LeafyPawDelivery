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

        void SetTotalProgress(int cnt);
    }

    public class DailyMissionCell : Base<DailyMissionCell.Data>, IDailyMission
    {
        public class Data : BaseData
        {
            public IListener iListener = null;

            public int id = 0;
            public int progress = 0;
            public int value = 0;

            public bool isTotal = false;
        }

        public interface IListener
        {
            void GetReward(int id);
        }

        [SerializeField]
        private TextMeshProUGUI titleTMP = null;
        [SerializeField]
        private Image progressImg = null;
        [SerializeField]
        private TextMeshProUGUI progressTMP = null;
        [SerializeField]
        private OpenConditionVertical openCondition = null;
        [SerializeField]
        private RectTransform completedRootRectTm = null;

        private float _progress = 0;

        public int Id { get { return _data != null ? _data.id : 0; } }

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
                localKey = "dailymission_" + _data.id;
                title = string.Format(LocalizationSettings.StringDatabase.GetLocalizedString("Acquire", localKey, LocalizationSettings.SelectedLocale), _data.value);
            }
           
            titleTMP?.SetText(title);
        }

        private void SetProgress()
        {
            if (_data == null)
                return;

            if (_data.isTotal)
                return;

            float dataProgress = DataProgress;
            int id = Id;

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

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                Text = Game.Data.Const.DailyMissionRewardCount.ToString(),
            };

            openCondition.Initialize(openConditionData);
        }

        private float DataProgress
        {
            get
            {
                return _data != null ? _data.value : 0;
            }
        }

        private bool GetRewarded
        {
            get
            {
                bool getRewarded = true;

                var acquireMgr = MainGameManager.Get<Game.Manager.Acquire>();
                if(acquireMgr != null)
                {
                    getRewarded = acquireMgr.GetRewardDailyMission(Id);
                }

                return getRewarded;
            }
        }

        private bool IsCompleted
        {
            get
            {
                if (_data == null)
                    return false;

                return _progress >= DataProgress;
            }
        }

        private void SetGetReward(bool getReward)
        {
            if (_data == null)
                return;

            var acquireMgr = MainGameManager.Get<Game.Manager.Acquire>();
            if (acquireMgr == null)
                return;

            acquireMgr.SetGetRewardDailyMission(_data.id, getReward);
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
            if (_data == null)
                return;

            SetGetReward(false);
        }

        void IDailyMission.SetTotalProgress(int progress)
        {
            _progress = progress;

            float dataProgress = DataProgress;
            
            progressImg.fillAmount = _progress / dataProgress;
            progressTMP?.SetText(_progress + " / " + dataProgress);
        }
        #endregion

        public void OnClickGetReward()
        {
            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);

            if (_data == null)
                return;

            var dataProgress = DataProgress;
            if (dataProgress <= 0)
                return;

            if (_progress < dataProgress)
                return;

            Game.UIManager.Instance?.Top?.CollectCashCurrency(openCondition.transform.position, Game.Data.Const.DailyMissionRewardCount);

            UIUtils.SetActive(completedRootRectTm, true);

            SetGetReward(true);

            if (Id <= 0)
                return;

            if (transform)
            {
                transform.SetAsLastSibling();
            }

            _data?.iListener?.GetReward(Id);
        }
    }
}

