using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace GameSystem
{
    public static class SceneLoader
    {
        public static void LoadWithLoading(Load.Base loadData)
        {
            Load(Type.EScene.Loading.ToString(),
                (handle) =>
                {
                    var loading = GameObject.FindObjectOfType<Loading>();
                    loading?.Init(handle.Result.Scene, loadData);
                });
        }

        public static void Load(string sceneName, System.Action<AsyncOperationHandle<SceneInstance>> completedAction)
        {
            AsyncOperationHandle<SceneInstance> sceneInstance = Addressables.LoadSceneAsync("Assets/Scenes/" + sceneName + "Scene.unity", LoadSceneMode.Additive);
            sceneInstance.Completed += completedAction;
        }
    }
}

