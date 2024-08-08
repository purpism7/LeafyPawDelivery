using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Localization.Settings;

using TMPro;
using System;

namespace UI.Component
{
    public class AchievementCell : Base<AchievementCell.Data>
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
            public int Id = 0;
            public List<Achievement> AchievementDataList = null;
        }

        public interface IListener
        {
            void GetReward();
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

        private int _step = 1;
        private float _progress = 0;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            SetOpenCondition();
            SetProgress();
        }

        public override void Activate()
        {
            base.Activate();

            Set();

            UIUtils.SetActive(completedRootRectTm, GetRewarded);
        }

        private void Set()
        {
            SetStep();
            SetTitleTMP();
            SetProgress();
        }

        private void SetStep()
        {
            int id = _data != null ? _data.Id : 0;
            var achievementInfo = MainGameManager.Get<Game.Manager.Acquire>()?.GetAchievement(id);

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
            var achievementInfo = MainGameManager.Get<Game.Manager.Acquire>()?.GetAchievement(_data.Id);

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

        private bool IsLastStep
        {
            get
            {
                var achievementDataList = _data?.AchievementDataList;
                if (achievementDataList == null)
                    return false;

                int lastStep = 0;
                foreach(var achievementData in achievementDataList)
                {
                    if (achievementData == null)
                        continue;

                    if (lastStep >= achievementData.Step)
                        continue;

                    lastStep = achievementData.Step;
                }

                return _step >= lastStep;
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

        private bool GetRewarded
        {
            get
            {
                if (_data == null)
                    return true;

                var acquireMgr = MainGameManager.Get<Game.Manager.Acquire>();
                if (acquireMgr == null)
                    return true;

                return acquireMgr.GetRewardAchievement(_data.Id);
            }
        }

        private void SetGetReward(bool getReward)
        {
            if (_data == null)
                return;

            var acquireMgr = MainGameManager.Get<Game.Manager.Acquire>();
            if (acquireMgr == null)
                return;

            acquireMgr.SetGetRewardAhchievement(_data.Id, getReward);
        }

        private void SetOpenCondition()
        {
            if (openCondition == null)
                return;

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                Text = Games.Data.Const.AchievementRewardCount.ToString(),
            };

            openCondition.Initialize(openConditionData);
        }

        public void OnClickGetReward()
        {
            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);

            var dataProgress = DataProgress;
            if (dataProgress <= 0)
                return;

            if (_progress < dataProgress)
                return;

            Game.UIManager.Instance?.Top?.CollectCashCurrency(openCondition.transform.position, Games.Data.Const.AchievementRewardCount);

            if(IsLastStep)
            {
                SetGetReward(true);

                UIUtils.SetActive(completedRootRectTm, GetRewarded);
            }

            if(_data != null)
            {
                MainGameManager.Get<Game.Manager.Acquire>()?.SetNextStep(_data.Id);
            }

            Set();

            _data?.iListener?.GetReward();
        }
    }
}

