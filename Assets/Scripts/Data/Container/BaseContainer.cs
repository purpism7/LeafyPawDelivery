using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseContainer
{
    public abstract void Init(string json);
}

public class BaseContainer<T> : BaseContainer
{
    protected static T[] _datas = null;

    public static T[] GetDatas 
    {
        get
        {
            return _datas;
        }
    }

    public override void Init(string json)
    {
        var wrapper = JsonHelper.WrapperFromJson<T>(json);
        if(wrapper != null)
        {
            _datas = wrapper.Datas;
        }
    }
}

