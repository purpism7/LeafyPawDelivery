using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plugin
{
    public class Native : Singleton<Native>
    {
        private Base _base = null;

        protected override void Initialize()
        {
            if(Application.isEditor)
            {
                _base = gameObject.GetOrAddComponent<Editor>();
            }
            else
            {
#if UNITY_IOS
                _base = gameObject.GetOrAddComponent<IOS>();
#elif UNITY_ANDROID
                _base = gameObject.GetOrAddComponent<Android>();
#else
                _base = gameObject.GetOrAddComponent<Editor>();
#endif
            }

            _base?.Initialize();
        }

        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());

            Debug.Log("end Native Initialize");
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

        public string GetString(string key)
        {
            if(!Application.isEditor)
            {
                if (string.IsNullOrEmpty(key))
                    return string.Empty;
            }

            return _base?.GetString(key);
        }
    }

    public abstract class Base : MonoBehaviour
    {
#if UNITY_EDITOR
        protected string _userInfoJsonFilePath = "Assets/Info/{0}/User.json";
#else
        protected string _userInfoJsonFilePath = Application.persistentDataPath + "/Info/{0}/User.json";
#endif

        public abstract void Initialize();

        public abstract void SetString(string key, string value);
        public abstract string GetString(string key);
    }
}

