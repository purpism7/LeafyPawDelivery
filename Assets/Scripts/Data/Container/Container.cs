using System.Collections;
using System.Collections.Generic;
using GameSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Container : GameSystem.Processing
    {
        // private static Dictionary<System.Type, BaseContainer> _dataDic = new();

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            // _dataDic.Clear();
            
            yield return StartCoroutine(CoLoadData());

            Debug.Log(GetType().Name);

            yield return null;
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
            {
                yield break;
            }

            yield return addressableAssetLoader.CoLoadAssetAsync<TextAsset>(addressableAssetLoader.AssetLabelData,
                (asyncOperationHandle) =>
                {
                    var result = asyncOperationHandle.Result;
                    if(result == null)
                    {
                        return;
                    }

                    var typeName = asyncOperationHandle.Result.name + "Container";
                    var type = System.Type.GetType(typeName);

                    if(type != null)
                    {
                        var container = System.Activator.CreateInstance(type) as BaseContainer;
                        container?.Init(result.text);
                        
                        // _dataDic.Add(type, container);

                        Debug.Log(typeName);
                    }
                });

            yield return null;
        }
    }

