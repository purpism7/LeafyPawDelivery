using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectContainer : BaseContainer<ObjectContainer, Object>
{
    public List<Object> GetDataListByPlaceId(int placeId)
    {
        if (_datas == null)
            return null;

        var dataList = new List<Object>();
        dataList?.Clear();

        foreach(var data in _datas)
        {
            if (data == null)
                continue;

            if (data.PlaceId != placeId)
                continue;

            dataList?.Add(data);
        }

        return dataList;
    }
}


