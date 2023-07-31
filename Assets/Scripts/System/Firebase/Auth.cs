using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Auth;


namespace GameSystem.Firebase
{
    public class Auth : MonoBehaviour
    {
        public string UserId { get; private set; } = string.Empty;

        private bool _endLoad = false;

        private AppOptions _authOptions = new AppOptions
        {
            ApiKey = "",
            AppId = "",
            ProjectId = ""
        };

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(UserId);
            }
        }

        public IEnumerator CoInit()
        {
            Debug.Log("Auth CoInit");
            //var auth = FirebaseAuth.DefaultInstance;
            
            var auth = FirebaseAuth.GetAuth(FirebaseApp.Create());
            var currUser = auth.CurrentUser;
            if(currUser != null)
            {
                UserId = currUser.UserId;
            }

            Debug.Log("UserId = " + UserId);

            // 익명으로 로그인 진행.
            if (string.IsNullOrEmpty(UserId))
            {
                auth?.SignInAnonymouslyAsync().ContinueWith(
                    (task) =>
                    {
                        Debug.Log("task = " + task.Result);
                        if (task.IsCanceled)
                            return;

                        if (task.IsFaulted)
                            return;

                        var result = task.Result;

                        UserId = result.User.UserId;
                        Debug.Log("UserId = " + UserId);

                        _endLoad = true;
                    });
            }
            else
            {
                _endLoad = true;
            }

            yield return new WaitUntil(() => _endLoad);
        }

        //private void OnStateChanged(object sender, System.EventArgs eventArgs)
        //{
        //    var auth = FirebaseAuth.DefaultInstance;
        //    var userId = auth.CurrentUser.UserId;
        //    Debug.Log("Auth State Changed = " + userId);
        //    PlayerPrefs.SetString("Auth_UserId", userId);

            
        //}
    }
}

