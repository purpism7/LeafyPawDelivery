using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Plugin.Windows
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    public class SystemTray : MonoBehaviour
    {
        private static SystemTray _instance;
        private IntPtr _trayIconHandle = IntPtr.Zero;
        private bool _isInitialized = false;

        // Win32 API 함수들
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;

        public static SystemTray Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("SystemTray");
                    _instance = go.AddComponent<SystemTray>();
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

            // Unity 윈도우 핸들 가져오기
            IntPtr unityWindow = GetUnityWindow();
            if (unityWindow != IntPtr.Zero)
            {
                _trayIconHandle = unityWindow;
                _isInitialized = true;
                Debug.Log("SystemTray initialized");
            }
            else
            {
                Debug.LogWarning("Failed to get Unity window handle");
            }
        }

        private IntPtr GetUnityWindow()
        {
            // Unity 윈도우 찾기
            IntPtr window = FindWindow(null, Application.productName);
            if (window == IntPtr.Zero)
            {
                // 다른 방법으로 시도
                window = GetActiveWindow();
            }
            return window;
        }

        public void MinimizeToTray()
        {
            if (!_isInitialized)
                Initialize();

            IntPtr window = GetUnityWindow();
            if (window != IntPtr.Zero)
            {
                ShowWindow(window, SW_MINIMIZE);
                Debug.Log("Window minimized to tray");
            }
        }

        public void HideWindow()
        {
            if (!_isInitialized)
                Initialize();

            IntPtr window = GetUnityWindow();
            if (window != IntPtr.Zero)
            {
                ShowWindow(window, SW_HIDE);
                Debug.Log("Window hidden");
            }
        }

        public void ShowWindow()
        {
            if (!_isInitialized)
                Initialize();

            IntPtr window = GetUnityWindow();
            if (window != IntPtr.Zero)
            {
                ShowWindow(window, SW_SHOW);
                Debug.Log("Window shown");
            }
        }

        public bool IsWindowVisible()
        {
            IntPtr window = GetUnityWindow();
            if (window != IntPtr.Zero)
            {
                return IsWindowVisible(window);
            }
            return false;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // 포커스 이벤트 처리
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            // 일시정지 이벤트 처리
        }

        private void OnDestroy()
        {
            _isInitialized = false;
            _trayIconHandle = IntPtr.Zero;
        }
    }
#endif
}
