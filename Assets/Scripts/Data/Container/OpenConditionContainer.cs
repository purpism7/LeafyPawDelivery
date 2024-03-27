using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OpenConditionContainer<T> : BaseContainer<T, OpenConditionData> where T : new()
{
    public bool Check(int id)
    {
        var data = GetData(id);
        if (data == null)
            return false;

        var user = Info.UserManager.Instance.User;
        if (user == null)
            return false;

        var currency = user.GetCurrency(GameUtils.ActivityPlaceId);
        if (currency == null)
            return false;

        if (data.AnimalCurrency > 0 &&
            currency.Animal < data.AnimalCurrency)
            return false;

        if (data.ObjectCurrency > 0 &&
            currency.Object < data.ObjectCurrency)
            return false;

        if (data.Cash > 0 &&
           user.Cash < data.Cash)
            return false;

        return true;
    }

    public bool CheckAnimalCurrency(int id)
    {
        var data = GetData(id);
        if (data == null)
            return false;

        var user = Info.UserManager.Instance.User;
        if (user == null)
            return false;

        var currency = user.CurrenctCurrency;
        if (currency == null)
            return false;

        return currency.Animal >= data.AnimalCurrency;
    }

    public bool CheckObjectCurrency(int id)
    {
        var data = GetData(id);
        if (data == null)
            return false;

        var user = Info.UserManager.Instance.User;
        if (user == null)
            return false;

        var currency = user.CurrenctCurrency;
        if (currency == null)
            return false;

        return currency.Object >= data.ObjectCurrency;
    }

    public bool CheckReq(int id)
    {
        var data = GetData(id);
        if (data == null)
            return false;

        if (!CheckAnimalReq(data.ReqAnimalIds))
            return false;

        if (!CheckObjectReq(data.ReqObjectIds))
            return false;

        return true;
    }

    protected bool CheckAnimalReq(int[] reqIds)
    {
        var animalMgr = MainGameManager.Get<Game.AnimalManager>();
        if (animalMgr == null)
            return false;

        if (reqIds != null)
        {
            foreach (int animalId in reqIds)
            {
                if (!animalMgr.CheckExist(animalId))
                    return false;
            }
        }

        return true;
    }

    protected bool CheckObjectReq(int[] reqIds)
    {
        var objectMgr = MainGameManager.Get<Game.ObjectManager>();
        if (objectMgr == null)
            return false;

        if (reqIds != null)
        {
            foreach (int objectId in reqIds)
            {
                if (!objectMgr.CheckExist(objectId))
                    return false;
            }
        }

        return true;
    }

    public List<OpenConditionData> GetDataList(OpenConditionData.EType[] eTypes)
    {
        if (eTypes == null)
            return null;

        if (_datas == null)
            return null;

        var dataList = new List<OpenConditionData>();
        dataList.Clear();

        foreach (var data in _datas)
        {
            if (data == null)
                continue;

            if (data.Id <= 0)
                continue;

            if (!eTypes.ToList().Contains(data.eType))
                continue;

            dataList.Add(data);
        }

        return dataList;
    }
}
