using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Statics<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static T Create()
    {
        if (_instance == null)
        {
            var gameObj = new GameObject(typeof(T).Name);
            if (!gameObj)
                return null;

            gameObj.transform.SetParent(MainGameManager.Instance?.transform);

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
            return Create();
        }
    }
}
