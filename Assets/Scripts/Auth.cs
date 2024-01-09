using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

using Cysharp.Threading.Tasks;

namespace GameSystem
{
    public class Auth
    {
        private static Auth _instance = null;

        public static string ID { get { return _instance?._id; } }
        public static string NickName { get { return _instance?._nickName; } }

        private readonly string KeyNickName = string.Empty;

        private string _id = string.Empty;
        private string _nickName = string.Empty;

        public Auth()
        {
            _instance = this;

            KeyNickName = GetType() + "NickName";
        }

        public async UniTask AsyncInitialize()
        {
            await UnityServices.InitializeAsync();
            Debug.Log(UnityServices.State);

            // 1. GameCenter / GPGS 로그인.
            Social.localUser.Authenticate(SocialAuthenticateCallback);

            _nickName = PlayerPrefs.GetString(KeyNickName, string.Empty);
        }

        private void SocialAuthenticateCallback(bool success)
        {
            if(success)
            {
                Debug.Log("Success Social Authenticate");
                Debug.Log("UserName = " + Social.localUser.userName);

                _id = Social.localUser.id;
            }
            else
            {
                AsyncSignInAnonymously().Forget();
            }
        }

        private async UniTask AsyncSignInAnonymously()
        {
            await AuthenticationService.Instance?.SignInAnonymouslyAsync();

            _id = AuthenticationService.Instance?.PlayerId;

            Debug.Log("AsyncSignInAnonymously = " + _id);
        }

        public void SetNickName(string nickName)
        {
            _nickName = nickName;

            PlayerPrefs.SetString(KeyNickName, nickName);
        }

        //public IEnumerator CoInitialize()
        //{
        //    yield return UnityServices.InitializeAsync();
        //    Debug.Log(UnityServices.State);


        //    // 1. GameCenter / GPGS 로그인.
        //    Social.localUser.Authenticate(
        //      (success) =>
        //      {
        //          if()
        //          Debug.Log("Success Social Authenticate");
        //           //GameCenterPlatform.Show(true);
        //           //Social.
        //          Debug.Log(Social.localUser.id);
        //          Debug.Log("UserName = " + Social.localUser.userName);
        //      });

        //    yield return AuthenticationService.Instance?.SignInAnonymouslyAsync();

        //    _id = AuthenticationService.Instance?.PlayerId;
        //    Debug.Log($"PlayerID: {_id}");

           
        //}
    }
}

