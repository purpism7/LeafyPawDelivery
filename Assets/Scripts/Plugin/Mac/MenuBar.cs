using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Plugin.Mac
{
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    public class MenuBar : MonoBehaviour
    {
        private static MenuBar _instance;
        private bool _isInitialized = false;

        // macOS 네이티브 함수들 (나중에 Objective-C 플러그인으로 구현)
        [DllImport("__Internal")]
        private static extern void _CreateMenuBarItem(string title);

        [DllImport("__Internal")]
        private static extern void _SetMenuBarItemTitle(string title);

        [DllImport("__Internal")]
        private static extern void _AddMenuItem(string title, string action);

        [DllImport("__Internal")]
        private static extern void _ShowWindow();

        [DllImport("__Internal")]
        private static extern void _HideWindow();

        [DllImport("__Internal")]
        private static extern bool _IsWindowVisible();

        public static MenuBar Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("MenuBar");
                    _instance = go.AddComponent<MenuBar>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
            try
            {
                _CreateMenuBarItem(Application.productName);
                _isInitialized = true;
                Debug.Log("MenuBar initialized");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to initialize MenuBar: {e.Message}");
            }
#else
            Debug.Log("MenuBar: Editor mode - initialization skipped");
            _isInitialized = true;
#endif
        }

        public void CreateMenuBarItem(string title)
        {
            if (!_isInitialized)
                Initialize();

#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
            try
            {
                _CreateMenuBarItem(title);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to create menu bar item: {e.Message}");
            }
#endif
        }

        public void AddMenuItem(string title, string action)
        {
            if (!_isInitialized)
                Initialize();

#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
            try
            {
                _AddMenuItem(title, action);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to add menu item: {e.Message}");
            }
#endif
        }

        public void ShowWindow()
        {
            if (!_isInitialized)
                Initialize();

#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
            try
            {
                _ShowWindow();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to show window: {e.Message}");
            }
#endif
        }

        public void HideWindow()
        {
            if (!_isInitialized)
                Initialize();

#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
            try
            {
                _HideWindow();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to hide window: {e.Message}");
            }
#endif
        }

        public bool IsWindowVisible()
        {
#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
            try
            {
                return _IsWindowVisible();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to check window visibility: {e.Message}");
                return true;
            }
#else
            return true;
#endif
        }

        private void OnDestroy()
        {
            _isInitialized = false;
        }
    }
#endif
}
