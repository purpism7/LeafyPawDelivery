using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementContainer : BaseContainer<AchievementContainer, Achievement>
{
    public Dictionary<int, List<Achievement>> AchievementListDic { get; private set; } = new();

    protected override void InternalInitialize()
    {
        base.InternalInitialize();

        AchievementListDic?.Clear();

        List<Achievement> dataList = null;

        foreach(var data in _datas)
        {
            if (data == null)
                continue;

            if(AchievementListDic.TryGetValue(data.Id, out dataList))
            {
                dataList.Add(data);
            }
            else
            {
                dataList = new();
                dataList.Clear();
                dataList.Add(data);

                AchievementListDic.TryAdd(data.Id, dataList);
            }
        }
    }

    public Achievement GetData(int step, Game.Type.EAcquire eAcquire, Game.Type.EAcquireAction eAcquireAction)
    {
        if (_datas == null)
            return null;

        foreach (var data in _datas)
        {
            if (data == null)
                continue;

            if (data.Step != step)
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
