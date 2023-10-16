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

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetDailyMissionList();
            SetAchievementList();
        }

        public override void Activate()
        {
            base.Activate();

            _currETabType = Type.ETab.DailyMission;

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
                      
                   })
                   .SetRootRectTm(dailyMissionScrollRect.content)
                   .Create();

                _dailyMissionCellList.Add(cell);
            }
        }

        private void SetAchievementList()
        {
            var achievementDatas = AchievementContainer.Instance?.Datas;
            if (achievementDatas == null)
                return;

            foreach(var achievementData in achievementDatas)
            {
                if (achievementData == null)
                    continue;
            }

        }
    }
}

