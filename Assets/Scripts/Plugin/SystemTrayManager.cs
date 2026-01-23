using System;
using UnityEngine;

namespace Plugin
{
    /// <summary>
    /// Windows와 macOS에서 시스템 트레이/메뉴바 인터랙션을 관리하는 매니저
    /// </summary>
    public class SystemTrayManager : Singleton<SystemTrayManager>
    {
        private bool _isInitialized = false;
        private bool _isMinimizedToTray = false;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private Windows.SystemTray _windowsTray = null;
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        private Mac.MenuBar _macMenuBar = null;
#endif

        protected override void Initialize()
        {
            if (_isInitialized)
                return;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            _windowsTray = Windows.SystemTray.Instance;
            _windowsTray.Initialize();
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            _macMenuBar = Mac.MenuBar.Instance;
            _macMenuBar.Initialize();
            _macMenuBar.CreateMenuBarItem(Application.productName);
            _macMenuBar.AddMenuItem("Show Window", "ShowWindow");
            _macMenuBar.AddMenuItem("Hide Window", "HideWindow");
            _macMenuBar.AddMenuItem("Quit", "Quit");
#endif

            // 백그라운드에서도 실행되도록 설정
            Application.runInBackground = true;

            _isInitialized = true;
            Debug.Log("SystemTrayManager initialized");
        }

        /// <summary>
        /// 윈도우를 트레이/메뉴바로 최소화
        /// </summary>
        public void MinimizeToTray()
        {
            if (!_isInitialized)
                Initialize();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            _windowsTray?.MinimizeToTray();
            _isMinimizedToTray = true;
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            _macMenuBar?.HideWindow();
            _isMinimizedToTray = true;
#endif
        }

        /// <summary>
        /// 윈도우를 숨김
        /// </summary>
        public void HideWindow()
        {
            if (!_isInitialized)
                Initialize();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            _windowsTray?.HideWindow();
            _isMinimizedToTray = true;
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            _macMenuBar?.HideWindow();
            _isMinimizedToTray = true;
#endif
        }

        /// <summary>
        /// 윈도우를 표시
        /// </summary>
        public void ShowWindow()
        {
            if (!_isInitialized)
                Initialize();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            _windowsTray?.ShowWindow();
            _isMinimizedToTray = false;
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            _macMenuBar?.ShowWindow();
            _isMinimizedToTray = false;
#endif
        }

        /// <summary>
        /// 윈도우 표시 상태 토글
        /// </summary>
        public void ToggleWindow()
        {
            if (_isMinimizedToTray)
            {
                ShowWindow();
            }
            else
            {
                MinimizeToTray();
            }
        }

        /// <summary>
        /// 윈도우가 표시되고 있는지 확인
        /// </summary>
        public bool IsWindowVisible()
        {
            if (!_isInitialized)
                return true;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            return _windowsTray?.IsWindowVisible() ?? true;
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            return _macMenuBar?.IsWindowVisible() ?? true;
#else
            return true;
#endif
        }

        /// <summary>
        /// macOS 메뉴바에 메뉴 아이템 추가
        /// </summary>
        public void AddMenuItem(string title, string action)
        {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            _macMenuBar?.AddMenuItem(title, action);
#endif
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // 포커스 이벤트 처리
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            // 일시정지 이벤트 처리
        }

        private void Update()
        {
            // 키보드 단축키로 윈도우 토글 (예: Ctrl+Shift+T)
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (Input.GetKeyDown(KeyCode.T))
                    {
                        ToggleWindow();
                    }
                }
            }
        }
    }
}
