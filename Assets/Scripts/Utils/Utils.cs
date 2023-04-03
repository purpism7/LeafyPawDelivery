using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

public static class Utils
{
    public static T Get<T>(GameObject gameObj) where T : MonoBehaviour
    {
        T t = default(T);

        if (!gameObj)
        {
            return t;
        }

        if (t == null)
        {
            t = gameObj.GetComponent<T>();
            if (t == null)
            {
                t = gameObj.AddComponent<T>();
            }
        }

        return t;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObj) where T : Component
    {
        var component = gameObj.GetComponent<T>();
        if (component == null)
        {
            component = gameObj.AddComponent<T>();
        }

        return component;
    }
}

