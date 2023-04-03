using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    [System.Serializable]
    public class Wrapper<T>
    {
        public T[] Datas;
    }

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);

        return wrapper.Datas;
    }

    public static Wrapper<T> WrapperFromJson<T>(string json)
    {
        return JsonUtility.FromJson<Wrapper<T>>(json);
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Datas = array;

        return JsonUtility.ToJson(wrapper);
    }
}
