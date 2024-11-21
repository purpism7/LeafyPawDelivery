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

            if (data.Id <= 0)
                continue;

            if (data.PlaceId != placeId)
                continue;

            dataList?.Add(data);
        }

        return dataList;
    }

    public int GetInteractionObjectId(int animalId)
    {
        if (_datas == null)
            return 0;
        
        foreach (var data in _datas)
        {
            if (data == null)
                continue;

            if (data.Id != animalId)
                continue;

            return data.InteractionId;
        }

        return 0;
    }
    
    public bool CheckExistInteraction(int objectId)
    {
        if (_datas == null)
            return false;
        
        foreach (var data in _datas)
        {
            if (data == null)
                continue;

            if (data.InteractionId == objectId)
                return true;
        }

        return false;
    }
}
