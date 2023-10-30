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

    public Achievement GetData(int id, int step)
    {
        if (AchievementListDic == null)
            return null;

        if(AchievementListDic.TryGetValue(id, out List<Achievement> achievementList))
        {
            if (achievementList == null)
                return null;

            return achievementList.Find(achievement => achievement.Step == step);
        }

        return null;
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

    public int GetDataId(Game.Type.EAcquire eAcquire, Game.Type.EAcquireAction eAcquireAction)
    {
        if (_datas == null)
            return 0;

        foreach (var data in _datas)
        {
            if (data == null)
                continue;

            if (data.EAcquireType != eAcquire)
                continue;

            if (data.EAcquireActionType != eAcquireAction)
                continue;

            return data.Id;
        }

        return 0;
    }
}
