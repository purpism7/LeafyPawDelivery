using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plugin
{
    public class Native : Singleton<Native>
    {
        private Base _base = null;

        public string SaveValue { get { return _base?.SaveValue; } }

        protected override void Initialize()
        {
            // 1. 플랫폼별 컴포넌트 할당 (전략 패턴)
#if UNITY_EDITOR
            _base = gameObject.GetOrAddComponent<Local>();
#elif UNITY_ANDROID
    var eLoginType = GameSystem.Auth.ELoginType;
    // 로그인 타입에 따라 분기하되, 기본값은 Local 등으로 방어 로직 구축
    if(eLoginType == GameSystem.Auth.EType.GooglePlayGames)
        _base = gameObject.GetOrAddComponent<Android>();
    else
        _base = gameObject.GetOrAddComponent<Local>(); 
#elif UNITY_IOS
    _base = gameObject.GetOrAddComponent<IOS>();
#else
    _base = gameObject.GetOrAddComponent<Local>();
#endif

            // 2. _base가 확실히 있을 때 Initialize 호출
            if (_base != null)
            {
                Debug.Log($"[Native] Initialized with {_base.GetType().Name}");
                _base.Initialize();
            }
            else
            {
                Debug.LogError("[Native] Failed to initialize Base component!");
            }
            
//             if (Application.isEditor)
//                 return;
//
//             var eLoginType = GameSystem.Auth.ELoginType;
//             if(eLoginType == GameSystem.Auth.EType.GooglePlayGames)
//             {
// #if UNITY_ANDROID
//                 _base = gameObject.GetOrAddComponent<Android>();
// #endif
//             }
//             else if(eLoginType == GameSystem.Auth.EType.GameCenter)
//             {
// #if UNITY_IOS
//                 _base = gameObject.GetOrAddComponent<IOS>();
// #endif
//             }
//             //else
//             //{
//             //    _base = gameObject.GetOrAddComponent<Local>();
//             //}
//             
//
// //            if(Application.isEditor)
// //            {
// //                _base = gameObject.GetOrAddComponent<Local>();
// //            }
// //            else
// //            {
// //#if UNITY_IOS
// //                _base = gameObject.GetOrAddComponent<IOS>();
// //#elif UNITY_ANDROID
// //                _base = gameObject.GetOrAddComponent<Android>();
// //#else
// //                _base = gameObject.GetOrAddComponent<Local>();
// //#endif
// //            }
//
//             _base?.Initialize();
        }

        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());
        }

        public void SetString(string key, string value)
        {
            if (!Application.isEditor)
            {
                if (string.IsNullOrEmpty(key))
                    return;
            }

            _base?.SetString(key, value);
        }

        public void GetString(string key, System.Action<bool, string> endAction)
        {
            if(!Application.isEditor)
            {
                if (string.IsNullOrEmpty(key))
                    return;
            }

            _base?.GetString(key, endAction);
        }
    }

    public abstract class Base : MonoBehaviour
    {
        //#if UNITY_EDITOR
        //        protected string _userInfoJsonFilePath = "Assets/Info/User.json";
        //#else
        //        protected string _userInfoJsonFilePath = Application.persistentDataPath + "/User.json";
        //#endif

        public string SaveValue { get; protected set; } = string.Empty;

        public abstract void Initialize();

        public abstract void SetString(string key, string value);
        public abstract void GetString(string key, System.Action<bool, string> endAction);
    }
}

