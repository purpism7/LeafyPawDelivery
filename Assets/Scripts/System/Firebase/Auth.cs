using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Auth;


namespace GameSystem.Firebase
{
    public class Auth : MonoBehaviour
    {
        public string UserId { get; private set; } = string.Empty;

        private bool _endLoad = false;

        public IEnumerator CoInit()
        {
            var auth = FirebaseAuth.DefaultInstance;
            var currUser = auth.CurrentUser;

            UserId = currUser.UserId;

            Debug.Log("UserId = " + UserId);
            if (!string.IsNullOrEmpty(UserId))
                yield break;

            //auth.StateChanged += OnStateChanged;

            yield return auth?.SignInAnonymouslyAsync().ContinueWith(
                task =>
                {
                    if (task.IsCanceled)
                        return;

                    if (task.IsFaulted)
                        return;

                    var result = task.Result;

                    UserId = result.User.UserId;
                    Debug.Log("UserId = " + UserId);

                    _endLoad = true;
                });


            while (!_endLoad)
            {
                yield return null;
            }
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

