using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenConditionContainer<T> : BaseContainer<T, OpenCondition> where T : new()
{
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
