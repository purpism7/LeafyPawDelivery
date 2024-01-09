using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

using Cysharp.Threading.Tasks;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

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

            //PlayGamesPlatform.Activate();

#if UNITY_ANDROID
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
#endif

            // 1. GameCenter / GPGS 로그인.
            //Social.localUser.Authenticate(SocialAuthenticateCallback);

            //await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync("");

            _nickName = PlayerPrefs.GetString(KeyNickName, string.Empty);
        }

        internal void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true,
                    (code) =>
                    {
                        //AuthenticationService.Instance.PlayerInfo.Id

                        SignInWithGooglePlayGameServiceAsync(code).Forget();

                    });
            }
            else
            {
                SignInAnonymouslyAsync().Forget();
            }
            Debug.Log("ProcessAuthentication = " + status);
        }

        //private void SocialAuthenticateCallback(bool success)
        //{
        //    if(success)
        //    {
        //        Debug.Log("Success Social Authenticate");
        //        Debug.Log("UserName = " + Social.localUser.userName);

        //        _id = Social.localUser.id;
        //    }
        //    else
        //    {
        //        AsyncSignInAnonymously().Forget();
        //    }
        //}

        private async UniTask SignWithAppleGameCenterAsync()
        {
            try
            {
                //await AuthenticationService.Instance.SignInWithAppleGameCenterAsync()
            }
            catch
            {

            }
        }

        // GPGS 로그인.
        private async UniTask SignInWithGooglePlayGameServiceAsync(string code)
        {
            try
            {
                await AuthenticationService.Instance?.SignInWithGooglePlayGamesAsync(code);

                SetId(AuthenticationService.Instance?.PlayerInfo?.GetGooglePlayGamesId());

                Debug.Log("SignInWithGooglePlayGameServiceAsync = " + _id);
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }
        }

        // 익명으로 로그인.
        private async UniTask SignInAnonymouslyAsync()
        {
            await AuthenticationService.Instance?.SignInAnonymouslyAsync();

            SetId(AuthenticationService.Instance?.PlayerId);

            Debug.Log("SignInAnonymouslyAsync = " + _id);
        }

        private void SetId(string id)
        {
            _id = id;
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

