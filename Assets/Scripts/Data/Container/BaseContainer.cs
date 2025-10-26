using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

public abstract class BaseContainer
{
    public abstract void Initialize(object obj, string jsonStr);
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

    public override void Initialize(object obj, string jsonStr)
    {
        _instance = (T)obj;

        if (typeof(T) == typeof(ItemContainer))
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            
            var datas = JsonConvert.DeserializeObject<V[]>(jsonStr, settings);
            _datas = datas;
            
            InternalInitialize();

            return;
        }
        
        var wrapper = JsonHelper.WrapperFromJson<V>(jsonStr);
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

