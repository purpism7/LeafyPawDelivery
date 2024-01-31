using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryOpenConditionContainer : BaseContainer<StoryOpenConditionContainer, StoryOpenCondition>
{
    public bool CheckReq(int id)
    {
        var data = GetData(id);
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

    public bool CheckExistReqId(int id, Game.Type.EElement eElement, int reqId)
    {
        var data = GetData(id);
        if (data == null)
            return false;

        if (eElement == Game.Type.EElement.Animal)
        {
            return CheckExistReqId(reqId, eElement, data.ReqAnimalIds);
        }
        else if(eElement == Game.Type.EElement.Object)
        {
            return CheckExistReqId(reqId, eElement, data.ReqObjectIds);
        }

        return false;
    }

    private bool CheckExistReqId(int reqId, Game.Type.EElement eElement, int[] ids)
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
