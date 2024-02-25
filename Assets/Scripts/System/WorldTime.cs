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
        struct TimeData
        {
            public string datetime;
        }

        public DateTime? DateTime { get; private set; } = null;
        public bool Sync { get; private set; } = false;

        private void Start()
        {
            RequestAsync().Forget();

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

                UnityWebRequest webRequest = new UnityWebRequest();

                using (webRequest = UnityWebRequest.Get(Game.Data.Const.WorldTimeURI))
                {
                    await webRequest.SendWebRequest();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        var timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);

                        if (string.IsNullOrEmpty(timeData.datetime))
                            return null;

                        if (System.DateTime.TryParse(timeData.datetime, out DateTime dateTime))
                        {
                            DateTime = dateTime;

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

