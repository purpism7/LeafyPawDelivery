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
            if (Application.isEditor)
            {
                _base = gameObject.GetOrAddComponent<Editor>();
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                _base = gameObject.GetOrAddComponent<Android>();
            }
            else
            {
                _base = gameObject.GetOrAddComponent<IOS>();
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
        public abstract void Initialize();

        public abstract void SetString(string key, string value);
        public abstract string GetString(string key);
    }
}

