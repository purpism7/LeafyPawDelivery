using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace GameSystem.Loader
{
    public static class Scene
    {
        public enum ESceneType
        {
            None,

            Loading,
            Logo,
            Login,
            Game,
        }

        //private ESceneType _eLoadSceneType = ESceneType.None;

        
        public static void Load(Load.Base load)    
        {
            //_eLoadSceneType = eSceneType;

            var activeScene = SceneManager.GetActiveScene();
            Debug.Log("ActiveScene = " + activeScene.name);

            LoadScene(load.SceneName, EndLoad);
        }

        public static void LoadWithLoading(Load.Base load)
        {
            var activeScene = SceneManager.GetActiveScene();
            Debug.Log("ActiveScene = " + activeScene.name);

            LoadScene(ESceneType.Loading.ToString(),
                (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        var loading = GameObject.FindObjectOfType<Loading>();
                        loading?.Init();

                        SceneManager.UnloadSceneAsync(activeScene);
                        Debug.Log(handle.Result.Scene.name);
                        LoadScene(ESceneType.Login.ToString(), EndLoad);
                    }
                });
        }

        private static void LoadScene(string sceneName, System.Action<AsyncOperationHandle<SceneInstance>> completedAction)
        {
            AsyncOperationHandle<SceneInstance> sceneInstance = Addressables.LoadSceneAsync("Assets/Scenes/" + sceneName + "Scene.unity", LoadSceneMode.Additive);
            sceneInstance.Completed += completedAction;
        }

        private static void EndLoad(AsyncOperationHandle<SceneInstance> handle)
        {
            var activeScene = SceneManager.GetActiveScene();
            Debug.Log("ActiveScene = " + activeScene.name); 
            //SceneManager.UnloadSceneAsync()
            Debug.Log(handle.Result.Scene.name);
        }
    }
}

