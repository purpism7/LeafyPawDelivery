using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace GameSystem
{
    public class ServerTime : MonoBehaviour
    {
        public DateTime? DatTime { get; private set; } = null;

        private void Start()
        {
            AsyncWebRequest().Forget();
        }

        private async UniTask AsyncWebRequest()
        {
            UnityWebRequest webRequest = new UnityWebRequest();

            using (webRequest = UnityWebRequest.Get("www.google.co.kr"))
            {
                await webRequest.SendWebRequest();

                if(webRequest.result == UnityWebRequest.Result.Success)
                {
                    string date = webRequest.GetResponseHeader("date");

                    //callback?.Invoke(date);
                    if(DateTime.TryParse(date, out DateTime dateTime))
                    {
                        Debug.Log("serverTime = " + (DateTime.UtcNow - dateTime).TotalSeconds);
                        DatTime = dateTime;
                        
                    }
                }
                else
                {

                }
            }
        }
    }
}

