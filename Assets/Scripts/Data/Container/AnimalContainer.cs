using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalContainer : BaseContainer<AnimalContainer, Animal>
{
    public List<Animal> GetDataListByPlaceId(int placeId)
    {
        if (_datas == null)
            return null;

        var dataList = new List<Animal>();
        dataList?.Clear();

        foreach (var data in _datas)
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
