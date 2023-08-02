using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


using GameSystem;
using GameSystem.Load;

namespace GameSystem
{
    public class Loading : MonoBehaviour, Scene.Base.IListener
    {
        #region Inspector
        public RectTransform loadingTypeRootRectTm = null;
        [SerializeField] private RectTransform rootRectTm = null;
        #endregion

        private UnityEngine.SceneManagement.Scene _scene;
        private Load.Base _loadData = null;
        private UnityEngine.SceneManagement.Scene _activeScene;
        
        public void Init(UnityEngine.SceneManagement.Scene scene, Load.Base loadData)
        {
            if (loadData == null)
            {
                return;
            }
            
            _scene = scene;
            _loadData = loadData;

            if (rootRectTm)
            {
                rootRectTm.gameObject.SetActive(loadData.ActiveLoading);
            }
           
            Load();
        }
        
        private void Load()
        {
            _activeScene = SceneManager.GetActiveScene();
            if(!_loadData.ActiveLoading)
            {
                //var cameraData = _loadData.MainCamera.GetUniversalAdditionalCameraData();
                //cameraData.
            }

            //SceneManager.UnloadSceneAsync(activeScene);
            SceneLoader.Load(_loadData.SceneName, EndLoad);
        }
        
        //private void LoadScene(string sceneName, System.Action<AsyncOperationHandle<SceneInstance>> completedAction)
        //{
        //    AsyncOperationHandle<SceneInstance> sceneInstance = Addressables.LoadSceneAsync("Assets/Scenes/" + sceneName + "Scene.unity", LoadSceneMode.Additive);
        //    sceneInstance.Completed += completedAction;
        //}
        
        private void EndLoad(AsyncOperationHandle<SceneInstance> handle)
        {
            SceneManager.UnloadSceneAsync(_activeScene);

            _activeScene = SceneManager.GetActiveScene();
            Debug.Log("ActiveScene = " + _activeScene.name); 
            //SceneManager.UnloadSceneAsync()
            Debug.Log(handle.Result.Scene.name);

            InitScene();
        }

        private void InitScene()
        {
            var initSceneType = System.Type.GetType("Scene." + _loadData.SceneName);
            var initScene = GameObject.FindObjectOfType(initSceneType) as Scene.Base;
            initScene?.Init(this);
        }

        void Scene.Base.IListener.EndLoad()
        {
            StartCoroutine(CoUnloadSceneAsync());
        }

        private IEnumerator CoUnloadSceneAsync()
        {
            yield return new WaitForEndOfFrame();
            
            SceneManager.UnloadSceneAsync(_scene);
        }
    }
}


