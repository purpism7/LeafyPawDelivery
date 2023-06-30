using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

using GameSystem.Load;
using UnityEngine.UI;

namespace Scene
{
    public class Login : Base
    {
        [SerializeField] private LoadData loadData = null;
        [SerializeField] private Button btn = null;

        public override void Init(IListener iListener)
        {
            base.Init(iListener);
            
            // SceneLoader.LoadWithLoading(loadData);

            _iListener?.EndLoad();
            
            StartCoroutine(CoInit());
        }

        private IEnumerator CoInit()
        {
            yield return StartCoroutine(FirebaseManager.Instance.CoInit());

            var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            yield return auth?.SignInAnonymouslyAsync().ContinueWith(task => {
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

                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                btn.interactable = true;

            });
        }

        public void OnClick()
        {
            SceneLoader.LoadWithLoading(loadData);
        }
    }
}