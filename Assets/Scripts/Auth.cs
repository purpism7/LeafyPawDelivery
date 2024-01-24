using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

using Cysharp.Threading.Tasks;

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

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
        public static EType ELoginType { get { return _instance._eType; } } 

        private readonly string KeyNickName = string.Empty;

        public enum EType
        {
            Local,
            GameCenter,
            GooglePlayGames,
        }

        private string _id = string.Empty;
        private string _nickName = string.Empty;
        private EType _eType = EType.Local;

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

            //            if(Application.isEditor)
            //            {
            //                await SignInAnonymouslyAsync();
            //            }
            //            else
            //            {
            //#if UNITY_IOS
            //                //var player = await GKLocalPlayer.Authenticate();
            //                //Debug.Log($"GameKit Authentication: player {player}");

            _nickName = PlayerPrefs.GetString(KeyNickName, string.Empty);

            if (Application.isEditor)
            {
                await SignInAnonymouslyAsync();
            }
            else if(Application.platform == RuntimePlatform.Android)
            {
                _eType = EType.GooglePlayGames;

#if UNITY_ANDROID
                PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                    .EnableSavedGames()
                    .RequestServerAuthCode(false)
                    .Build();

                PlayGamesPlatform.InitializeInstance(config);
                PlayGamesPlatform.DebugLogEnabled = true;
                PlayGamesPlatform.Activate();

                PlayGamesPlatform.Instance.Authenticate(SocialAuthenticateCallback);
#endif
            }
            else
            {
                _eType = EType.GameCenter;

                Social.localUser.Authenticate(SocialAuthenticateCallback);

            }
            //            }

            //Social.Active.Authenticate()
            // 1. GameCenter / GPGS 로그인.
            

            //await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync("");
        }

#if UNITY_ANDROID
        //internal void ProcessAuthentication(SignInStatus status)
        //{
        //    if (status == SignInStatus.Success)
        //    {
        //        PlayGamesPlatform.Instance.RequestServerSideAccess(true,
        //            (code) =>
        //            {
        //                //AuthenticationService.Instance.PlayerInfo.Id

        //                SignInWithGooglePlayGameServiceAsync(code).Forget();

        //            });
        //    }
        //    else
        //    {
        //        SignInAnonymouslyAsync().Forget();
        //    }
        //    Debug.Log("ProcessAuthentication = " + status);
        //}
#endif

        private void SocialAuthenticateCallback(bool success, string error)
        {
            if (success)
            {
                Debug.Log("Success Social Authenticate");
                Debug.Log("UserName = " + Social.localUser.userName);

                SetId(Social.localUser.id);
                //_id = Social.localUser.id;

                Debug.Log("SocialAuthenticateCallback = " + _id);
            }
            else
            {
                Debug.Log(error);

                SignInAnonymouslyAsync().Forget();
            }
        }

        //private async UniTask SignWithAppleGameCenterAsync()
        //{
        //    try
        //    {
        //        //await AuthenticationService.Instance.SignInWithAppleGameCenterAsync()
        //    }
        //    catch
        //    {

        //    }
        //}

        // GPGS 로그인.
        //private async UniTask SignInWithGooglePlayGameServiceAsync(string code)
        //{
        //    try
        //    {
        //        await AuthenticationService.Instance?.SignInWithGooglePlayGamesAsync(code);

        //        SetId(AuthenticationService.Instance?.PlayerInfo?.GetGooglePlayGamesId());

        //        Debug.Log("SignInWithGooglePlayGameServiceAsync = " + _id);
        //    }
        //    catch (AuthenticationException ex)
        //    {
        //        Debug.LogException(ex);
        //    }
        //    catch (RequestFailedException ex)
        //    {
        //        Debug.LogException(ex);
        //    }
        //}

        // 익명으로 로그인.
        private async UniTask SignInAnonymouslyAsync()
        {
            _eType = EType.Local;

            //await AuthenticationService.Instance?.SignInAnonymouslyAsync();

            //SetId(AuthenticationService.Instance?.PlayerId);
            SetId(AuthenticationService.Instance?.PlayerId);

            await UniTask.Yield();

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

