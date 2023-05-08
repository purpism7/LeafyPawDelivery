using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace GameSystem.Loader
{
    public class Scene
    {
        public enum ESceneType
        {
            None,

            Loading,
            Logo,
            Login,
            Game,
        }

        private ESceneType _eLoadSceneType = ESceneType.None;

        private AssetReference _assetRef;

        public Scene Load(ESceneType eSceneType)
        {
            //_eLoadSceneType = eSceneType;

            var activeScene = SceneManager.GetActiveScene();
            Debug.Log("ActiveScene = " + activeScene.name);

            var value = Addressables.LoadSceneAsync(eSceneType.ToString() + "Scene");
            
            LoadScene(ESceneType.Loading,
                (operation) =>
                {
                    LoadScene(eSceneType, EndLoad);
                });
            //var operation = SceneManager.LoadSceneAsync(ESceneType.Loading.ToString() + "Scene", LoadSceneMode.Additive);
            //if (operation == null)
            //{
            //    return;
            //}

            //operation.completed += EndLoad;

            return this;
        }

        private void LoadScene(ESceneType eSceneType, System.Action<AsyncOperationHandle<SceneInstance>> completedAction)
        {
            UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(eSceneType.ToString() + "Scene", LoadSceneMode.Additive)
                .Completed += completedAction;
        }

        private void EndLoad(AsyncOperationHandle<SceneInstance> operation)
        {
            var activeScene = SceneManager.GetActiveScene();
            Debug.Log("ActiveScene = " + activeScene.name); 
            //SceneManager.UnloadSceneAsync()
        }
    }
}

