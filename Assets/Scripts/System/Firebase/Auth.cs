using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Auth;


namespace GameSystem.Firebase
{
    public class Auth : MonoBehaviour
    {
        public string UserId { get; private set; } = string.Empty;

        public IEnumerator CoInit()
        {
            UserId = PlayerPrefs.GetString("Auth_UserId");
            Debug.Log("UserId = " + UserId);
            if(string.IsNullOrEmpty(UserId))
            {
                bool endLoad = false;

                var auth = FirebaseAuth.DefaultInstance;
                yield return auth?.SignInAnonymouslyAsync().ContinueWith(
                    task => {
                        if (task.IsCanceled)
                        {
                            Debug.LogError("SignInAnonymouslyAsync was canceled.");
                            return;
                        }
                        if (task.IsFaulted)
                        {
                            Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                            return;
                        }

                        var result = task.Result;

                        UserId = result.User.UserId;

                        PlayerPrefs.SetString("Auth_UserId", UserId);

                        endLoad = true;
                    });


                while(!endLoad)
                {
                    yield return null;
                }
                //yield return new WaitUntil(() => endLoad);
            }

            Debug.Log(UserId);
        }
    }
}

