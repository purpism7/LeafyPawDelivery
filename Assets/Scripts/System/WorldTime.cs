using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

using Cysharp.Threading.Tasks;

namespace GameSystem
{
    public class WorldTime : MonoBehaviour
    {
        #region Static
        private static WorldTime _instance = null;
        public static WorldTime Create()
        {
            if (_instance == null)
            {
                var gameObj = new GameObject(typeof(WorldTime).Name);
                if (!gameObj)
                    return null;

                _instance = gameObj.GetOrAddComponent<WorldTime>();
            }

            if (_instance != null)
            {
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }

        public static bool Validate
        {
            get { return _instance != null; }
        }

        public static WorldTime Get
        {
            get
            {
                if(_instance == null)
                {
                    Create();
                }

                return _instance;
            }
        }
        #endregion

        struct TimeData
        {
            public string datetime;
        }

        public DateTime? DateTime { get; private set; } = null;
        public bool Sync { get; private set; } = false;

        private void Start()
        {
            //RequestAsync().Forget();

            //LocalTime = DateTime.UtcNow.ToLocalTime();
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                RequestAsync().Forget();
            }
            else
            {
                Sync = false;
            }
        }

        public async UniTask<DateTime?> RequestAsync()
        {
            try
            {
                Sync = false;

                //using ()
                using (UnityWebRequest webRequest = UnityWebRequest.Get(Games.Data.Const.WorldTimeURI))   
                {
                    if (webRequest == null)
                        return null;

                    await webRequest.SendWebRequest();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        var timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);

                        if (string.IsNullOrEmpty(timeData.datetime))
                            return null;

                        if (System.DateTime.TryParse(timeData.datetime, out DateTime dateTime))
                        {
                            DateTime = dateTime;

                            //Debug.Log("RequestAsync = " + dateTime);
                            Sync = true;

                            return dateTime;
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }
    }
}

