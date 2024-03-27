using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOpenConditionContainer : OpenConditionContainer<ObjectOpenConditionContainer>
{
    protected override void InternalInitialize()
    {
        base.InternalInitialize();

        foreach(var data in _datas)
        {
            if (data == null)
                continue;

            if (data.Id <= 0)
                continue;

            data.ObjectCurrency = Mathf.FloorToInt(data.ObjectCurrency * 1.2f);
        }
    }

    public bool CheckPossibleBuy(out int id)
    {
        id = 0;

        var dataList = GetDataList(new[] { OpenConditionData.EType.Buy });
        if (dataList == null)
            return false;

        int placeId = GameUtils.ActivityPlaceId;

        var objectDataList = ObjectContainer.Instance?.GetDataListByPlaceId(placeId);
        if (objectDataList == null)
            return false;

        for (int i = 0; i < objectDataList.Count; ++i)
        {
            var objectData = objectDataList[i];
            if (objectData == null)
                continue;

            if (dataList.Find(data => data.Id == objectData.Id) == null)
                continue;

            if (CheckPossibleBuy(objectData.Id))
            {
                id = objectData.Id;

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

        if (mainGameMgr.CheckExist(Game.Type.EElement.Object, data.Id))
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
