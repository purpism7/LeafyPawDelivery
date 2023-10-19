using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DailyMissionContainer : BaseContainer<DailyMissionContainer, DailyMission>
{
    public DailyMission GetData(Game.Type.EAcquire eAcquire, Game.Type.EAcquireAction eAcquireAction)
    {
        if (_datas == null)
            return null;

        foreach(var data in _datas)
        {
            if (data == null)
                continue;

            if (data.EAcquireType != eAcquire)
                continue;

            if (data.EAcquireActionType != eAcquireAction)
                continue;

            return data;
        }

        return null;
    }
}
