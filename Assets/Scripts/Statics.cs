using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Statics<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static Transform _parentTm = null;
    public static T Create(Transform parentTm = null)
    {
        if (_instance == null)
        {
            var gameObj = new GameObject(typeof(T).Name);
            if (!gameObj)
                return null;

            var tm = gameObj.transform;
            if (!tm)
                return null;

            tm.SetParent(parentTm);

            tm.localPosition = Vector3.zero;
            tm.localScale = Vector3.one;

            _instance = gameObj.GetOrAddComponent<T>();
        }

        return _instance;
    }

    public static bool Validate
    {
        get { return _instance != null; }
    }

    public static T Get
    {
        get
        {
            return _instance;
        }
    }
}
