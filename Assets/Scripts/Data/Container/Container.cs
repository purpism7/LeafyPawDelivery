using System;
using System.Collections;
using GameData;
using GameSystem;
using UI;
using UnityEngine;

public class Container : GameSystem.Processing
{
    public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
    {
        yield return StartCoroutine(CoLoadData());
    }

    // public static T Get<T>() where T : BaseContainer
    // {
    //     if (_dataDic.TryGetValue(typeof(T), out BaseContainer container))
    //     {
    //         return container as T;
    //     }
    //     
    //     return default(T);
    // }

    private IEnumerator CoLoadData()
    {
        var addressableAssetLoader = GameSystem.ResourceManager.Instance?.AddressableAssetLoader;
        if(addressableAssetLoader == null)
            yield break;

        bool endLoad = false;

        yield return StartCoroutine(addressableAssetLoader.CoLoadAssetAsync<TextAsset>(addressableAssetLoader.AssetLabelData,
            (asyncOperationHandle) =>
            {
                var result = asyncOperationHandle.Result;
                if(result == null)
                    return;

                var typeName = asyncOperationHandle.Result.name + "Container";
                var type = System.Type.GetType(typeName);
               
                if(type != null)
                {
                    var container = System.Activator.CreateInstance(type);
                    var baseContainer = container as BaseContainer;
                    
                    baseContainer?.Init(container, result.text);
                }

                endLoad = true;
            }));

        yield return new WaitUntil(() => endLoad);
    }
}

