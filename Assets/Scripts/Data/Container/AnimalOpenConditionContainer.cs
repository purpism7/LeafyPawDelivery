using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalOpenConditionContainer : OpenConditionContainer<AnimalOpenConditionContainer>
{
    public bool CheckPossibleBuy(out int id)
    {
        id = 0;

        var dataList = GetDataList(new[] { OpenConditionData.EType.Buy });
        if (dataList == null)
            return false;

        int placeId = GameUtils.ActivityPlaceId;

        var animalDataList = AnimalContainer.Instance?.GetDataListByPlaceId(placeId);
        if (animalDataList == null)
            return false;

        for (int i = 0; i < animalDataList.Count; ++i)
        {
            var animalData = animalDataList[i];
            if (animalData == null)
                continue;

            if (dataList.Find(data => data.Id == animalData.Id) == null)
                continue;

            if (CheckPossibleBuy(animalData.Id))
            {
                id = animalData.Id;

                return true;
            }
        }

        return false;
    }

    public bool CheckPossibleBuy(int id)
    {
        var mainGameMgr = MainGameManager.Instance;
        if (mainGameMgr == null)
            return false;

        var data = GetData(id);
        if (data == null)
            return false;

        if (mainGameMgr.CheckExist(Game.Type.EElement.Animal, data.Id))
            return false;

        if (CheckAnimalReq(data.ReqAnimalIds) &&
           CheckObjectReq(data.ReqObjectIds))
        {
            if (CheckAnimalCurrency(data.Id) &&
               CheckObjectCurrency(data.Id))
            {
                return true;
            }
        }

        return false;
    }
}
