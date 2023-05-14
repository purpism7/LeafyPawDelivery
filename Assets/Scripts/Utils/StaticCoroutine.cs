using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoroutine : MonoBehaviour
{
    private static StaticCoroutine instance = null;

    public static void Create()
    {
        var gameObj = new GameObject("StaticCoroutine");
        instance = gameObj.GetOrAddComponent<StaticCoroutine>();

        DontDestroyOnLoad(gameObj);
    }

    public static void Start(IEnumerator enumerator)
    {
        if (instance == null)
        {
            Create();
        }

        instance?.StartCoroutine(enumerator);
    }
}
