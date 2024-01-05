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
    public interface IAchievement
    {
        bool IsCompleted { get; }
        bool GetRewarded { get; }
    }

    public class AchievementCell : Base<AchievementCell.Data>, IAchievement
    {
        public class Data : BaseData
        {
            public IListener iListener = null;
            public int Id = 0;
            public List<Achievement> AchievementDataList = null;
        }

        private const string KeyGetRewardedAchievement = "KeyGetRewardedAchievement_{0}";

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
        //[SerializeField]
        //private Button getRewardBtn = null;
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
                bool getRewarded = true;
                if (_data == null)
                    return getRewarded;

                if (System.Boolean.TryParse(PlayerPrefs.GetString(string.Format(KeyGetRewardedAchievement, _data.Id), false.ToString()), out getRewarded))
                {
                    return getRewarded;
                }

                return getRewarded;
            }
        }

        private void SetOpenCondition()
        {
            if (openCondition == null)
                return;

            var openConditionData = new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                Text = Game.Data.Const.AchievementRewardCount.ToString(),
            };

            openCondition.Initialize(openConditionData);
        }

        #region IAchievement
        bool IAchievement.IsCompleted
        {
            get
            {
                return IsCompleted;
            }
        }

        bool IAchievement.GetRewarded
        {
            get
            {
                return GetRewarded;
            }
        }
        #endregion

        public void OnClickGetReward()
        {
            var dataProgress = DataProgress;
            if (dataProgress <= 0)
                return;

            if (_progress < dataProgress)
                return;

            Game.UIManager.Instance?.Top?.CollectCashCurrency(openCondition.transform.position, Game.Data.Const.AchievementRewardCount);

            if(IsLastStep)
            {
                PlayerPrefs.SetString(string.Format(KeyGetRewardedAchievement, _data.Id), true.ToString());

                UIUtils.SetActive(completedRootRectTm, GetRewarded);

                //if (transform)
                //{
                //    transform.SetAsLastSibling();
                //}
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

