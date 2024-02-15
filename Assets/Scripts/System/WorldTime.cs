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

        public DateTime? LocalTime { get; private set; } = null;

        private void Start()
        {
            AsyncWebRequest().Forget();

            LocalTime = DateTime.UtcNow.ToLocalTime();
        }

        private async UniTask AsyncWebRequest()
        {
            UnityWebRequest webRequest = new UnityWebRequest();

            using (webRequest = UnityWebRequest.Get("https://worldtimeapi.org/api/ip"))
            {
                await webRequest.SendWebRequest();

                if(webRequest.result == UnityWebRequest.Result.Success)
                {
                    var timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);

                    if (string.IsNullOrEmpty(timeData.datetime))
                        return;
                    
                    if(DateTime.TryParse(timeData.datetime, out DateTime dateTime))
                    {
                        Debug.Log("time = " + timeData.datetime);
                        LocalTime = dateTime.ToLocalTime();
                    }
                }
                else
                {

                }
            }
        }
    }
}

