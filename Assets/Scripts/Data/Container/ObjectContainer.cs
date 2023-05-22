using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectContainer : BaseContainer<ObjectContainer, Object>
{
    public Object GetData(int id)
    {
        if (_datas == null)
            return null;

        foreach (var data in _datas)
        {
            if(data == null)
                continue;

            if (data.Id == id)
                return data;
        }

        return null;
    }
}


