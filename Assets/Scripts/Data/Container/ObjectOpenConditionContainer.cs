using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOpenConditionContainer : OpenConditionContainer<ObjectOpenConditionContainer>
{
    protected override void InternalInitialize()
    {
        base.InternalInitialize();

        foreach(var data in _datas)
        {
            if (data == null)
                continue;

            if (data.Id <= 0)
                continue;

            data.ObjectCurrency = Mathf.FloorToInt(data.ObjectCurrency * 1.2f);
        }
    }
}
