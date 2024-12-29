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
        public static EGameType EGameType_ { get { return _instance._eGameType; } }

        public enum EType
        {
            Local,
            GameCenter,
            GooglePlayGames,
        }

        public enum EGameType
        {
            None,

            New,
            Continue,
        }

        private string _id = string.Empty;
        private string _nickName = string.Empty;
        private EType _eType = EType.Local;
        private EGameType _eGameType = EGameType.None;

        private bool _endAuth = false;

        public Auth()
        {
            _instance = this;
        }

        public async UniTask AsyncInitialize()
        {
            await UnityServices.InitializeAsync();
            Debug.Log(UnityServices.State);

            _nickName = PlayerPrefs.GetString(Games.Data.PlayPrefsKeyNickName, string.Empty);

            _endAuth = false;
#if UNITY_ANDROID
            _eType = EType.GooglePlayGames;

            
            
            // PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //     .EnableSavedGames()
            //     //.RequestServerAuthCode(false)
            //     .Build();

            // PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            // PlayGamesPlatform.Instance.Authenticate(SocialAuthenticateCallback);
            PlayGamesPlatform.Instance.Authenticate(
                (SignInStatus status) =>
                {
                    if (status == SignInStatus.Success)
                    {
                        SetId(Social.localUser.id);
                    }

                    _endAuth = true;
                });
#else
            _eType = EType.GameCenter;

            Social.localUser.Authenticate(SocialAuthenticateCallback);
#endif

            await UniTask.WaitUntil(() => _endAuth);
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

                Debug.Log("SocialAuthenticateCallback = " + _id);
            }
            else
            {
                Debug.Log(error);

                //SignInAnonymouslyAsync().Forget();
            }

            _endAuth = true;
        }

        // 익명으로 로그인.
        //private async UniTask SignInAnonymouslyAsync()
        //{
        //    _eType = EType.Local;

        //    //await AuthenticationService.Instance?.SignInAnonymouslyAsync();

        //    SetId(AuthenticationService.Instance?.PlayerId);

        //    await UniTask.Yield();

        //    Debug.Log("SignInAnonymouslyAsync = " + _id);
        //}

        private void SetId(string id)
        {
            _id = id;
        }

        public void SetNickName(string nickName)
        {
            _nickName = nickName;

            PlayerPrefs.SetString(Games.Data.PlayPrefsKeyNickName, nickName);
        }

        public void SetGameType(EGameType eGameType)
        {
            _eGameType = eGameType;
        }
    }
}

