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
            UserId = PlayerPrefs.GetString("Auth_UserId", string.Empty);
            Debug.Log("UserId = " + UserId);
            if(!string.IsNullOrEmpty(UserId))
                yield break;

            bool endLoad = false;

            var auth = FirebaseAuth.DefaultInstance;
            yield return auth?.SignInAnonymouslyAsync().ContinueWith(
                task => {
                    if (task.IsCanceled)
                        return;

                    if (task.IsFaulted)
                        return;

                    var result = task.Result;

                    UserId = result.User.UserId;
                    Debug.Log("UserId = " + UserId);
                    PlayerPrefs.SetString("Auth_UserId", UserId);

                    endLoad = true;
                });


            while (!endLoad)
            {
                yield return null;
            }

            Debug.Log("End Load");

            Debug.Log(UserId);
        }
    }
}

