using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

using GameSystem;
using GameSystem.Load;

namespace GameSystem
{
    public class Loading : MonoBehaviour, Scene.Base.IListener
    {
        #region Inspector
        public RectTransform loadingTypeRootRectTm = null;
        [SerializeField]
        private RectTransform rootRectTm = null;
        [SerializeField]
        private TextMeshProUGUI loadingTMP = null;
        [SerializeField]
        private Image loadingProgressImg = null;
        #endregion

        private UnityEngine.SceneManagement.Scene _scene;
        private Load.Base _loadData = null;
        private UnityEngine.SceneManagement.Scene _activeScene;

        private float _progress = 0;
        private Queue<Tuple<float, float>> _progressQueue = null;
        
        public void Init(UnityEngine.SceneManagement.Scene scene, Load.Base loadData)
        {
            if (loadData == null)
                return;

            _scene = scene;
            _loadData = loadData;

            if (rootRectTm)
            {
                rootRectTm.gameObject.SetActive(loadData.ActiveLoading);
            }
           
            Load();

            SetProgress(0);
            TypingLoadingAsync().Forget();
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
            var initScene = GameObject.FindAnyObjectByType(initSceneType) as Scene.Base;
            initScene?.Init(this);
        }

        private IEnumerator CoUnloadSceneAsync()
        { 
            SceneManager.UnloadSceneAsync(_scene);

            yield return null;
        }

        private async UniTask TypingLoadingAsync()
        {
            var text = "Loading ...";

            loadingTMP?.SetText(string.Empty);

            foreach (var textChar in text)
            {
                loadingTMP?.SetText(loadingTMP.text + textChar);

                await UniTask.WaitForSeconds(0.2f);
            }

            await UniTask.Yield();

            TypingLoadingAsync().Forget();
        }

        private void SetProgress(float progress)
        {
            if (loadingProgressImg == null)
                return;

            loadingProgressImg.fillAmount = progress;

            _progress = progress;
        }

        private async UniTask ProgressAsync()
        {
            if(_progressQueue.TryDequeue(out Tuple<float, float > resProgress))
            {
                float progress = 0;
                while (resProgress.Item1 > progress)
                {
                    progress += 0.01f;
                    SetProgress(_progress + 0.01f);

                    await UniTask.WaitForSeconds(resProgress.Item2);
                }
            }

            if(_progressQueue.Count > 0)
            {
                ProgressAsync().Forget();
            }
        }

        #region Scene.Base.IListener
        void Scene.Base.IListener.Progress(float progress, float delay)
        {
            if(_progressQueue == null)
            {
                _progressQueue = new();
                _progressQueue.Clear();
            }

            _progressQueue.Enqueue(new (progress, delay));

            ProgressAsync().Forget();
        }

        void Scene.Base.IListener.EndLoad()
        {
            StartCoroutine(CoUnloadSceneAsync());
        }
        #endregion
    }
}


