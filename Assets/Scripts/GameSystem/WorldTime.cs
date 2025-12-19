using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net;

using Cysharp.Threading.Tasks;

namespace GameSystem
{
    public class WorldTime : MonoBehaviour
    {
        #region Static
        private static WorldTime _instance = null;
        private static WorldTime Create()
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
                    Create();

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

        //public async UniTask<DateTime?> RequestAsync()
        //{
        //    Sync = false;

        //    var worldTimeUri = Games.Data.Const?.WorldTimeURI;
        //    if (string.IsNullOrEmpty(worldTimeUri))
        //        return null;

        //    using (UnityWebRequest webRequest = UnityWebRequest.Get(worldTimeUri))   
        //    {
        //        if (webRequest == null)
        //            return null;

        //        try
        //        {
        //            // webRequest.certificateHandler = new CustomCertificateHandler();
        //            await webRequest.SendWebRequest();

        //            if (webRequest.result == UnityWebRequest.Result.Success)
        //            {
        //                var timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);
        //                if (string.IsNullOrEmpty(timeData.datetime))
        //                    return null;

        //                if (System.DateTime.TryParse(timeData.datetime, out DateTime dateTime))
        //                {
        //                    DateTime = dateTime;

        //                    //Debug.Log("RequestAsync = " + dateTime);
        //                    Sync = true;

        //                    return dateTime;
        //                }
        //            }
        //            else
        //            {
        //                Debug.Log("Fail");
        //                DateTime = System.DateTime.Now;

        //                Sync = true;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //            DateTime = System.DateTime.Now;

        //            Sync = true;
        //        }
        //    }

        //    return null;
        //}

        public async UniTask<DateTime?> RequestAsync()
        {
            Sync = false;

            var worldTimeUri = Games.Data.Const?.WorldTimeURI;
            if (string.IsNullOrEmpty(worldTimeUri))
            {
                Debug.LogError("WorldTime URI is empty.");
                return null;
            }

            using (UnityWebRequest webRequest = UnityWebRequest.Get(worldTimeUri))
            {
                try
                {
                    await webRequest.SendWebRequest();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        var timeData = JsonUtility.FromJson<TimeData>(webRequest.downloadHandler.text);

                        // 데이터 유효성 검사
                        if (!string.IsNullOrEmpty(timeData.datetime) &&
                            System.DateTime.TryParse(timeData.datetime, out DateTime dateTime))
                        {
                            DateTime = dateTime;
                            Sync = true;
                            // Debug.Log($"Server Time Synced: {dateTime}");
                            return dateTime; // 성공 시 서버 시간 반환
                        }
                    }

                    // 통신은 성공했으나 데이터가 이상하거나, 통신 실패인 경우
                    Debug.LogWarning($"Time Sync Failed: {webRequest.error}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Time Sync Exception: {e.Message}");
                }
            }

            // --- 실패 시 처리 (Fallback) ---
            // 정책에 따라 선택하세요.

            // 방법 1: 실패해도 로컬 시간을 쓰고, 리턴도 로컬 시간을 준다. (유연함)
            DateTime = System.DateTime.Now;
            Sync = true;
            return DateTime;

            // 방법 2: 실패하면 null을 리턴해서 호출한 곳에서 재시도 팝업을 띄우게 한다. (엄격함)
            // return null; 
        }
    }
}

