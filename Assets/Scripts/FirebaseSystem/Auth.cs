using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
//using Firebase;
//using Firebase.Auth;

namespace FirebaseSystem
{
    public class Auth : Base
    {
        public string UserId { get; private set; } = string.Empty;
        public bool IsFirst { get; private set; } = false;

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(UserId);
            }
        }

        public override async UniTask AsyncInitialize()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

                //var credentialAsync = Firebase.Auth.GameCenterAuthProvider.GetCredentialAsync();
                //var continueTask = credentialAsync.ContinueWith(
                //    (task) =>
                //    {
                //        Debug.Log("AsyncInitialize");
                //        if (task.IsCanceled)
                //        {
                //            Debug.LogError("SignInWithCredentialAsync was canceled.");
                //            return;
                //        }

                //        if (task.IsFaulted)
                //        {
                //            Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                //            return;
                //        }

                //        //Debug.Log(task.Result.Provider);
                //        var credential = task.Result;
                //        var credentialTask = FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential);

                //        credentialTask.ContinueWith(HandleSignInWithUser);

                //    });
            }

            await UniTask.CompletedTask;
        }

        //Called when a sign-in without fetching profile data completes.
        //void HandleSignInWithUser(Task<Firebase.Auth.FirebaseUser> task)
        //{
        //    //EnableUI();
        //    if (LogTaskCompletion(task, "Sign-in"))
        //    {
        //        Debug.Log(String.Format("{0} signed in", task.Result.DisplayName));
        //    }
        //}

        // Log the result of the specified task, returning true if the task
        // completed successfully, false otherwise.
        //bool LogTaskCompletion(Task task, string operation)
        //{
        //    bool complete = false;
        //    if (task.IsCanceled)
        //    {
        //        Debug.Log(operation + " canceled.");
        //    }
        //    else if (task.IsFaulted)
        //    {
        //        Debug.Log(operation + " encounted an error.");
        //        foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
        //        {
        //            string authErrorCode = "";
        //            Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
        //            if (firebaseEx != null)
        //            {
        //                authErrorCode = String.Format("AuthError.{0}: ",
        //                  ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
        //            }
        //            Debug.Log(authErrorCode + exception.ToString());
        //        }
        //    }
        //    else if (task.IsCompleted)
        //    {
        //        Debug.Log(operation + " completed");
        //        complete = true;
        //    }
        //    return complete;
        //}

        public override IEnumerator CoInit()
        {
            yield return null;
            //AsyncInitialize().Forget();

            //var auth = FirebaseAuth.GetAuth(FirebaseApp.Create());

            //var currUser = auth.CurrentUser;
            //if(currUser != null)
            //{
            //    UserId = currUser.UserId;
            //}

            //Debug.Log("UserId = " + UserId);

            //// 익명으로 로그인 진행.
            //if (string.IsNullOrEmpty(UserId))
            //{
            //    auth?.SignInAnonymouslyAsync().ContinueWith(
            //        (task) =>
            //        {
            //            Debug.Log("task = " + task.Result);
            //            if (task.IsCanceled)
            //                return;

            //            if (task.IsFaulted)
            //                return;

            //            var result = task.Result;

            //            UserId = result.User.UserId;
            //            Debug.Log("UserId = " + UserId);

            //            IsFirst = true;

            //            _endLoad = true;
            //        });
            //}
            //else
            //{
            //    _endLoad = true;
            //}

            //yield return new WaitUntil(() => _endLoad);
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

