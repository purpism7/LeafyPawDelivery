using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        var animalMgr = MainGameManager.Get<Game.AnimalManager>();
        if (animalMgr == null)
            return false;

        var objectMgr = MainGameManager.Get<Game.ObjectManager>();
        if (objectMgr == null)
            return false;

        var reqIds = data.ReqAnimalIds;
        if (reqIds != null)
        {
            foreach (int animalId in reqIds)
            {
                if(!animalMgr.CheckExist(animalId))
                    return false;
            }
        }

        reqIds = data.ReqObjectIds;
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
}
