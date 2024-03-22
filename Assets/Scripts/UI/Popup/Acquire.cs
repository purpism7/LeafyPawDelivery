using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using TMPro;
using DG.Tweening;

using Game;

namespace UI
{
    public class Acquire : BasePopup<Acquire.Data>, Component.DailyMissionCell.IListener, Component.AchievementCell.IListener
    {
        public class Data : BaseData
        {

        }

        [SerializeField]
        private Toggle[] tabToggles = null;
        [SerializeField]
        private RectTransform[] tabRedDotRectTms = null;
        [SerializeField]
        private ScrollRect dailyMissionScrollRect = null;
        [SerializeField]
        private ScrollRect achievementsScrollRect = null;

        [Header("Daily Mission")]
        [SerializeField]
        private RectTransform dailyMissionRootRectTm = null;
        [SerializeField]
        private TextMeshProUGUI localRemainTimeTMP = null;
        [SerializeField]
        private Component.DailyMissionCell totalDailyMissionCell = null;

        private Type.ETab _currETabType = Type.ETab.DailyMission;
        private List<Component.DailyMissionCell> _dailyMissionCellList = new();
        private List<Component.AchievementCell> _achievementCellList = new();

        private bool _resetDailyMission = false;

        private Game.Manager.Acquire _acquireMgr = null;

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            _acquireMgr = MainGameManager.Get<Game.Manager.Acquire>();

            _dailyMissionCellList?.Clear();
            _achievementCellList?.Clear();

            SetDailyMissionList();
            SetAchievementList();

            InitializeChildComponent();

            yield return null;
        }

        public override void Activate()
        {
            base.Activate();

            _resetDailyMission = false;

            if (_acquireMgr != null)
            {
                if (_acquireMgr.CheckResetDailyMission)
                {
                    ResetDailyMission();
                }
            }

            _currETabType = Type.ETab.DailyMission;
            ActiveContents();

            var tabToggle = tabToggles?.First();
            if (tabToggle != null)
            {
                tabToggle.SetIsOnWithoutNotify(true);
            }

            SetNotification();
            SetTotalProgress();
        }

        public override void Deactivate()
        {
            base.Deactivate();            
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            UpdateRemainTime();
        }

        private void UpdateRemainTime()
        {
            if (_acquireMgr == null)
                return;

            var localRemainTime = _acquireMgr.DailyMissionDateTime.Value - System.DateTime.UtcNow.ToLocalTime();

            localRemainTimeTMP?.SetText(localRemainTime.ToString(@"hh\:mm\:ss"));
            
            if (!_resetDailyMission &&
                _acquireMgr.CheckResetDailyMission)
            {
                _resetDailyMission = true;

                ResetDailyMission();
            }
        }

        private void SetDailyMissionList()
        {
            var dailyMissions = DailyMissionContainer.Instance?.Datas;
            if (dailyMissions == null)
                return;

            foreach (var dailyMission in dailyMissions)
            {
                if (dailyMission == null)
                    continue;

                var cell = new GameSystem.ComponentCreator<Component.DailyMissionCell, Component.DailyMissionCell.Data>()
                   .SetData(new Component.DailyMissionCell.Data()
                   {
                       iListener = this,
                       id = dailyMission.Id,
                       value = dailyMission.Value,
                   })
                   .SetRootRectTm(dailyMissionScrollRect.content)
                   .Create();

                _dailyMissionCellList.Add(cell);
            }

            foreach (var dailyMissionCell in _dailyMissionCellList)
            {
                if (dailyMissionCell == null)
                    continue;

                var iDailyMission = dailyMissionCell as UI.Component.IDailyMission;
                if (iDailyMission == null)
                    continue;

                if (!iDailyMission.GetRewarded)
                    continue;

                if (!dailyMissionCell.transform)
                    continue;

                dailyMissionCell.transform.SetAsLastSibling();
            }

            totalDailyMissionCell?.Initialize(
                new Component.DailyMissionCell.Data()
                {
                    //progress = _dailyMissionCellList.Count,
                    value = _dailyMissionCellList.Count,

                    isTotal = true,
                });
            totalDailyMissionCell?.Activate();
        }

        private void ResetDailyMission()
        {
            if (_dailyMissionCellList == null)
                return;

            _acquireMgr?.ResetDailyMission();

            foreach (var cell in _dailyMissionCellList)
            {
                if (cell == null)
                    continue;

                ResetDailyMissionCell(cell);

                cell.transform.SetSiblingIndex(cell.Id);
            }

            ResetDailyMissionCell(totalDailyMissionCell);

            SetNotification();
            SetTotalProgress();
        }

        private void ResetDailyMissionCell(Component.IDailyMission iDailyMission)
        {
            if (iDailyMission == null)
                return;

            iDailyMission.Reset();
            //var iDailyMission = cell as Component.IDailyMission;
            //if (iDailyMission == null)
            //    return;

            //iDailyMission.Reset();
        }

        private void SetAchievementList()
        {
            var achievementListDic = AchievementContainer.Instance?.AchievementListDic;
            if (achievementListDic == null)
                return;

            foreach (var pair in achievementListDic)
            {
                var cell = new GameSystem.ComponentCreator<Component.AchievementCell, Component.AchievementCell.Data>()
                  .SetData(new Component.AchievementCell.Data()
                  {
                      iListener = this,
                      Id = pair.Key,
                      AchievementDataList = pair.Value,
                  })
                  .SetRootRectTm(achievementsScrollRect.content)
                  .Create();

                _achievementCellList.Add(cell);
            }
        }

        private void ActiveContents()
        {
            ActivateChildComponent(_currETabType == Game.Type.ETab.DailyMission ? typeof(Component.DailyMissionCell) : typeof(Component.AchievementCell));

            UIUtils.SetActive(dailyMissionRootRectTm, _currETabType == Game.Type.ETab.DailyMission);
            UIUtils.SetActive(achievementsScrollRect?.gameObject, _currETabType == Game.Type.ETab.Achievement);

            dailyMissionScrollRect?.ResetScrollPos();
            achievementsScrollRect?.ResetScrollPos();
        }

        private void SetTotalProgress()
        {
            if (_dailyMissionCellList == null)
                return;

            int progress = 0;

            foreach (Component.IDailyMission iDailMission in _dailyMissionCellList)
            {
                if (iDailMission == null)
                    continue;

                if (iDailMission.GetRewarded)
                {
                    ++progress;

                    continue;
                }
            }

            (totalDailyMissionCell as Component.IDailyMission).SetTotalProgress(progress);
        }

        private void SetNotification()
        {
            SetDailyMissionNotification();
            SetAchievementNotification();
        }

        private void SetDailyMissionNotification()
        {
            if (tabRedDotRectTms == null)
                return;

            bool notification = false;
            if (_acquireMgr != null)
            {
                notification = _acquireMgr.CheckDailyMissionNotification;
            }

            UIUtils.SetActive(tabRedDotRectTms.First(), notification);

            if (!notification)
            {
                Info.Connector.Get?.SetCompleteDailyMission(false);
            }
        }

        private void SetAchievementNotification()
        {
            if (tabRedDotRectTms == null)
                return;

            bool notification = false;
            if (_acquireMgr != null)
            {
                notification = _acquireMgr.CheckAchievementNotification;
            }

            UIUtils.SetActive(tabRedDotRectTms.Last(), notification);

            if(!notification)
            {
                Info.Connector.Get?.SetCompleteAchievement(false);
            }
        }

        //private bool CheckDailyMissionNotification
        //{
        //    get
        //    {
        //        if (_dailyMissionCellList == null)
        //            return false;

        //        foreach (Component.IDailyMission iDailMission in _dailyMissionCellList)
        //        {
        //            if (iDailMission == null)
        //                continue;

        //            if (iDailMission.GetRewarded)
        //                continue;

        //            if (iDailMission.IsCompleted)
        //                return true;
        //        }

        //        var iTotalDailyMission = totalDailyMissionCell as Component.IDailyMission;
        //        if (iTotalDailyMission == null)
        //            return false;

        //        if (iTotalDailyMission.GetRewarded)
        //            return false;

        //        return iTotalDailyMission.IsCompleted;

        //    }
        //}

        //private bool CheckAchievementNotification
        //{
        //    get
        //    {
        //        if (_achievementCellList == null)
        //            return false;

        //        foreach (Component.IAchievement iAchievement in _achievementCellList)
        //        {
        //            if (iAchievement == null)
        //                continue;

        //            if (iAchievement.GetRewarded)
        //                continue;

        //            if (iAchievement.IsCompleted)
        //                return true;
        //        }

        //        return false;
        //    }
        //}

        //private void SetCompleteDailyMission()
        //{
        //    var connector = Info.Connector.Get;
        //    if (connector == null)
        //        return;

        //    UIUtils.SetActive(tabRedDotRectTms.First(), connector.CompleteDailyMission > 0);
        //}

        //private void SetCompleteAchievement()
        //{
        //    var connector = Info.Connector.Get;
        //    if (connector == null)
        //        return;

        //    UIUtils.SetActive(tabRedDotRectTms.Last(), connector.CompleteAchievement > 0);
        //}

        #region Component.DailyMissionCell.IListener
        void Component.DailyMissionCell.IListener.GetReward(int id)
        {
            SetTotalProgress();
            SetDailyMissionNotification();
        }
        #endregion

        #region Component.AchievementCell.IListener
        void Component.AchievementCell.IListener.GetReward()
        {
            //bool isNotification = CheckAchievementNotification;
            //UIUtils.SetActive(tabRedDotRectTms.Last(), isNotification);

            //Info.Connector.Get?.SetCompleteAchievement(isNotification);
            SetAchievementNotification();
        }
        #endregion

        public void OnChanged(string tabType)
        {
            if (System.Enum.TryParse(tabType, out Game.Type.ETab eTabType))
            {
                if (_currETabType == eTabType)
                    return;

                _currETabType = eTabType;

                ActiveContents();

                GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
            }
        }
    }
}

