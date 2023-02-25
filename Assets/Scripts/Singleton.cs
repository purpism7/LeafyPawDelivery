using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = FindObjectOfType<T>();
                if (obj == null)
                {
                    var gameObj = new GameObject(typeof(T).Name);
                    _instance = gameObj.AddComponent<T>();
                }
                else
                {
                    _instance = obj;
                }
            }

            return _instance;
        }
    }

    public abstract IEnumerator CoInit();
}

