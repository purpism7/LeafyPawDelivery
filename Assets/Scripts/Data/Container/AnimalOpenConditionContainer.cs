using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalOpenConditionContainer : OpenConditionContainer<AnimalOpenConditionContainer, OpeCondition>
{
    public bool Check(int id)
    {
        var data = GetData(id);
        if (data == null)
            return false;

        var user = Info.UserManager.Instance.User;
        if (user == null)
            return false;

        int activityPlaceId = MainGameManager.Instance.placeMgr.ActivityPlaceId;

        var currency = user.GetCurrency(activityPlaceId);
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

        return currency.Animal > data.AnimalCurrency;
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

        return currency.Object > data.ObjectCurrency;
    }
}
