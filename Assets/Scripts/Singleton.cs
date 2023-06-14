using System.Collections;
using System.Collections.Generic;
using GameSystem;
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
                Debug.Log(typeof(T).Name);

                var obj = GameObject.FindObjectOfType<T>();
                if (obj == null)
                {
                    var gameObj = new GameObject(typeof(T).Name);
                    _instance = gameObj.AddComponent<T>();
                }
                else
                {
                    _instance = obj;
                }
                
                _instance.GetComponent<Singleton<T>>()?.Initialize();
            }

            return _instance;
        }
    }
    
    public static bool Validate()
    {
        return _instance != null;
    }

    public virtual IEnumerator CoInit()
    {
        DontDestroyOnLoad(this);

        yield break;
    }

    public virtual IEnumerator CoInit(IPreprocessingProvider iProvider)
    {
        DontDestroyOnLoad(this);

        yield break;
    }
    
    protected abstract void Initialize();
}

