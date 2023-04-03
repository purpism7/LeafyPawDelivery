using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class OpenConditionManager : GameSystem.Processing
    {
        private List<Condition> _conditionList = new();

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            //var animalDataList = GameManager.Instance?.DataContainer?.AnimalDataList;
            //if(animalDataList != null)
            //{
            //    System.Action<int> openAction = null;
            //    var activityAnimalMgr = iProvider?.Get<ActivityAnimalManager>();
            //    if (activityAnimalMgr != null)
            //    {
            //        openAction = activityAnimalMgr.CreateActivityAnimal;
            //    }

            //    foreach(var animalData in animalDataList)
            //    {
            //        if(animalData == null)
            //        {
            //            continue;
            //        }

            //        var animalOpenCondition = new AnimalOpenConditionCreator()
            //            .SetAnimalId(animalData.Id)
            //            .SetOpenCondition(animalData.OpenCondition)
            //            .SetOpenAction(openAction)
            //            .Create();

            //        _conditionList.Add(animalOpenCondition);
            //    }
            //}

            yield break;
        }

        public void UpdaeCheckConditionList()
        {
            if(_conditionList == null)
            {
                return;
            }

            foreach(var condition in _conditionList)
            {
                if(condition == null)
                {
                    continue;
                }

                condition.Check();
            }
        }
    }
}
