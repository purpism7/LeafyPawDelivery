using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryOpenConditionContainer : BaseContainer<StoryOpenConditionContainer, StoryOpenCondition>
{
    private StoryOpenCondition GetDataByPlaceId(int id, int placeId)
    {
        if (_datas == null)
            return null;

        foreach (var data in _datas)
        {
            if (data == null)
                continue;

            if (data.Id <= 0)
                continue;

            if (data.PlaceId != placeId)
                continue;

            if (data.Id != id)
                continue;

            return data;
        }

        return null;
    }

    public bool CheckReq(int id, int placeId)
    {
        var data = GetDataByPlaceId(id, placeId);
        if (data == null)
            return false;

        if(!CheckReq(Game.Type.EElement.Animal, data.ReqAnimalIds))
            return false;

        if (!CheckReq(Game.Type.EElement.Object, data.ReqObjectIds))
            return false;

        return true;
    }

    private bool CheckReq(Game.Type.EElement eElement, int[] ids)
    {
        var mainGameMgr = MainGameManager.Instance;
        if (mainGameMgr == null)
            return false;

        if (ids != null)
        {
            foreach (int id in ids)
            {
                if (id <= 0)
                    continue;

                if (!mainGameMgr.CheckExist(eElement, id))
                    return false;
            }
        }

        return true;
    }

    public bool CheckExistReqId(int id, int placeId, Game.Type.EElement eElement, int reqId)
    {
        var data = GetDataByPlaceId(id, placeId);
        if (data == null)
            return false;

        if (eElement == Game.Type.EElement.Animal)
        {
            return CheckExistReqId(reqId, data.ReqAnimalIds);
        }
        else if(eElement == Game.Type.EElement.Object)
        {
            return CheckExistReqId(reqId, data.ReqObjectIds);
        }

        return false;
    }

    private bool CheckExistReqId(int reqId, int[] ids)
    {
        if (ids != null)
        {
            foreach (int id in ids)
            {
                if (id <= 0)
                    continue;

                if (reqId == id)
                    return true;
            }
        }

        return false;
    }
}
