using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSkinContainer : BaseContainer<AnimalSkinContainer, AnimalSkin>
{
    public List<AnimalSkin> GetSkinList(int animalId)
    {
        if (_datas == null)
            return null;

        List<AnimalSkin> skinList = new();
        skinList.Clear();

        foreach(var data in _datas)
        {
            if (data == null)
                continue;

            if(data.AnimalId == animalId)
            {
                skinList.Add(data);
            }
        }

        return skinList;
    }

    public AnimalSkin GetData(int id, int animalId)
    {
        foreach (var data in _datas)
        {
            if (data == null)
                continue;

            if (data.Id != id)
                continue;

            if (data.AnimalId != animalId)
                continue;

            return data;
        }

        return null;
    }

    public AnimalSkin GetBaseData(int animalId)
    {
        foreach (var data in _datas)
        {
            if (data == null)
                continue;

            if (data.AnimalId != animalId)
                continue;

            if (data.EAnimalSkin != Game.Type.EAnimalSkin.Base)
                continue;

            return data;
        }

        return null;
    }

    public int GetCurrency(int id, int animalId)
    {
        var animalData = AnimalContainer.Instance?.GetData(animalId);
        if (animalData == null)
            return 0;

        var data = GetData(id, animalId);
        if (data == null)
            return animalData.Currency;

        return animalData.Currency + data.Bonus;
    }
}
