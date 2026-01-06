using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class ObjectContainer : BaseContainer<ObjectContainer, Data.Object>
{
    public List<Data.Object> GetDataListByPlaceId(int placeId)
    {
        if (_datas == null)
            return null;

        var dataList = new List<Data.Object>();
        dataList?.Clear();

        foreach(var data in _datas)
        {
            if (data == null)
                continue;

            if (data.Id <= 0)
                continue;

            if (data.ObjectType != Type.ObjectType.Garden)
            {
                if (data.PlaceId != placeId)
                    continue;
            }

            dataList?.Add(data);
        }

        return dataList;
    }
}


