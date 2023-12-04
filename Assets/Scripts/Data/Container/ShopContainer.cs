using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopContainer : BaseContainer<ShopContainer, Data.Shop>
{
    public Dictionary<Game.Type.ECategory, List<Data.Shop>> ShopListDic = new();

    protected override void InternalInitialize()
    {
        base.InternalInitialize();

        ShopListDic?.Clear();

        List<Data.Shop> dataList = null;

        foreach(var data in _datas)
        {
            if (data == null)
                continue;

            if (data.ECategory == Game.Type.ECategory.None)
                continue;

            if (ShopListDic.TryGetValue(data.ECategory, out dataList))
            {
                dataList.Add(data);
            }
            else
            {
                dataList = new();
                dataList.Clear();
                dataList.Add(data);

                ShopListDic.TryAdd(data.ECategory, dataList);
            }
        }
    }
}
