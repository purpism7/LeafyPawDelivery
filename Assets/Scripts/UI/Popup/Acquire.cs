using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using Game;

namespace UI
{
    public class Acquire : BasePopup<Acquire.Data>
    {
        public class Data : BaseData
        {

        }

        [SerializeField] private Toggle[] tabToggles = null;
        [SerializeField] private ScrollRect dailyMissionScrollRect = null;
        [SerializeField] private ScrollRect achievementsScrollRect = null;

        private Type.ETab _currETabType = Type.ETab.DailyMission;
        private List<Component.DailyMissionCell> _dailyMissionCellList = new();
        private List<Component.AchievementCell> _achievementCellList = new();

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            _dailyMissionCellList?.Clear();
            _achievementCellList?.Clear();

            SetDailyMissionList();
            SetAchievementList();

            InitializeComponent();
            yield return null;
        }

        public override void Activate()
        {
            base.Activate();

            _currETabType = Type.ETab.DailyMission;
            ActiveContents();

            var tabToggle = tabToggles?.First();
            if (tabToggle != null)
            {
                tabToggle.SetIsOnWithoutNotify(true);
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
                       DailyMissionData = dailyMission,
                   })
                   .SetRootRectTm(dailyMissionScrollRect.content)
                   .Create();

                _dailyMissionCellList.Add(cell);
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
                      AchievementList = pair.Value,
                  })
                  .SetRootRectTm(achievementsScrollRect.content)
                  .Create();

                _achievementCellList.Add(cell);
            }
        }

        private void ActiveContents()
        {
            UIUtils.SetActive(dailyMissionScrollRect?.gameObject, _currETabType == Game.Type.ETab.DailyMission);
            UIUtils.SetActive(achievementsScrollRect?.gameObject, _currETabType == Game.Type.ETab.Achievement);
        }

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

