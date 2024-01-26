using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseContainer
{
    public abstract void Initialize(object obj, string json);
}

public class BaseContainer<T, V> : BaseContainer where T : new() where V : Data.Base
{
    private static T _instance = default(T);
    
    protected V[] _datas = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();

            return _instance;
        }
    }

    public V[] Datas 
    {
        get
        {
            return _datas;
        }
    }

    public override void Initialize(object obj, string json)
    {
        _instance = (T)obj;

        var wrapper = JsonHelper.WrapperFromJson<V>(json);
        if (wrapper != null)
        {
            var datas = wrapper.Datas;
            if(datas != null)
            {
                foreach(Data.Base dataBase in datas)
                {
                    dataBase?.Initialize();
                }
            }

            _datas = datas;

            InternalInitialize();
        }
    }

    protected virtual void InternalInitialize()
    {

    }

    public V GetData(int id)
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

