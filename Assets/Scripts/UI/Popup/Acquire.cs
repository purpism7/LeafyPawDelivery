using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using TMPro;

using Game;

namespace UI
{
    public class Acquire : BasePopup<Acquire.Data>, Component.DailyMissionCell.IListener
    {
        public class Data : BaseData
        {

        }

        [SerializeField] private Toggle[] tabToggles = null;
        [SerializeField] private ScrollRect dailyMissionScrollRect = null;
        [SerializeField] private ScrollRect achievementsScrollRect = null;

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

            SetTotalProgress();
        }

        private void Update()
        {
            var localRemainTime = System.DateTime.Today.AddDays(1).Subtract(System.DateTime.UtcNow);

            localRemainTimeTMP?.SetText(localRemainTime.ToString(@"hh\:mm\:ss"));
            
            if (!_resetDailyMission &&
                _acquireMgr != null && _acquireMgr.CheckResetDailyMission)
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

            foreach(var dailyMissionCell in _dailyMissionCellList)
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
            Debug.Log("ResetDailyMissionList");

            if (_dailyMissionCellList == null)
                return;

            MainGameManager.Get<Game.Manager.Acquire>()?.ResetDailyMission();

            foreach (var dailyMissionCell in _dailyMissionCellList)
            {
                if (dailyMissionCell == null)
                    continue;

                (dailyMissionCell as UI.Component.IDailyMission)?.Reset();

                dailyMissionCell?.Activate();

                dailyMissionCell.transform.SetSiblingIndex(dailyMissionCell.Id);
            }
        }

        private void SetAchievementList()
        {
            var achievementListDic = AchievementContainer.Instance?.AchievementListDic;
            if (achievementListDic == null)
                return;

            foreach(var pair in achievementListDic)
            {
                var cell = new GameSystem.ComponentCreator<Component.AchievementCell, Component.AchievementCell.Data>()
                  .SetData(new Component.AchievementCell.Data()
                  {
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

            foreach(Component.IDailyMission iDailMission in _dailyMissionCellList)
            {
                if (iDailMission == null)
                    continue;

                if (iDailMission.GetRewarded)
                {
                    ++progress;
                }
            }

            (totalDailyMissionCell as Component.IDailyMission).SetTotalProgress(progress);
        }

        #region Component.DailyMissionCell.IListener
        void Component.DailyMissionCell.IListener.GetReward(int id)
        {
            SetTotalProgress();
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
            }
        }
    }
}

