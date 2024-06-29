using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : BaseContainer<ItemContainer, Item>
{
    public List<Item> GetDataList(Game.Type.EItemSub eItemSub)
    {
        if (_datas == null)
            return null;

        var list = new List<Item>();
        list.Clear();
        
        foreach (var data in _datas)
        {
            if(data == null)
                continue;

            if (data.EItemSub == eItemSub)
            {
                list.Add(data);
            }
        }

        return list;
    }
}