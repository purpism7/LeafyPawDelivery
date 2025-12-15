using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

using Cysharp.Threading.Tasks;

#if UNITY_IOS
using Apple.GameKit;
#endif

using UnityEngine.SocialPlatforms.GameCenter;

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
            Debug.Log("UnityServices.State = " + UnityServices.State);

            _nickName = PlayerPrefs.GetString(Games.Data.PlayPrefsKeyNickName, string.Empty);

            _endAuth = false;
#if UNITY_ANDROID
            _eType = EType.GooglePlayGames;
            
            // PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //     .EnableSavedGames()
            //     //.RequestServerAuthCode(false)
            //     .Build();

            // PlayGamesPlatform.InitializeInstance(config);
            // PlayGamesPlatform.Instance.Authenticate(SocialAuthenticateCallback);
            
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

             PlayGamesPlatform.Instance?.Authenticate(
                 (status =>
                 {
                     Debug.Log(status);
                     if(status == SignInStatus.Success)
                     {
                         _endAuth = true;
                     }
                 }));

            // Social.localUser?.Authenticate(SocialAuthenticateCallback);
            // PlayGamesPlatform.Instance?.Authenticate(null, ProcessAuthentication);
#elif UNITY_IOS
            _eType = EType.GameCenter;
            
            try
            {
                if (!GKLocalPlayer.Local.IsAuthenticated)
                {
                    // Perform the authentication.
                    var player = await GKLocalPlayer.Authenticate();
                    Debug.Log($"GameKit Authentication: player {player}");
                }

                if (GKLocalPlayer.Local.IsAuthenticated)
                {
                    // Grab the display name.
                    var localPlayer = GKLocalPlayer.Local;
                    Debug.Log($"Local Player: {localPlayer.DisplayName}");

                    // Fetch the items.
                    var fetchItemsResponse =  await GKLocalPlayer.Local.FetchItems();

                    var signature = Convert.ToBase64String(fetchItemsResponse.GetSignature());
                    var teamPlayerID = localPlayer.TeamPlayerId;
                    Debug.Log($"Team Player ID: {teamPlayerID}");

                    var salt = Convert.ToBase64String(fetchItemsResponse.GetSalt());
                    var publicKeyUrl = fetchItemsResponse.PublicKeyUrl;
                    ulong timestamp = fetchItemsResponse.Timestamp;

                    Debug.Log($"GameKit Authentication: signature => {signature}");
                    Debug.Log($"GameKit Authentication: publickeyurl => {publicKeyUrl}");
                    Debug.Log($"GameKit Authentication: salt => {salt}");
                    Debug.Log($"GameKit Authentication: Timestamp => {timestamp}");
                
                    await AuthenticationService.Instance.SignInWithAppleGameCenterAsync(signature, teamPlayerID, publicKeyUrl, salt, timestamp);
                    Debug.Log("SignIn is successful.");
                    _endAuth = true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                _endAuth = true;
            }
#else
            _endAuth = true;
#endif

            //Social.localUser.Authenticate(SocialAuthenticateCallback);

            await UniTask.WaitUntil(() => _endAuth);
        }

#if UNITY_ANDROID
        // private void ProcessAuthentication(SignInStatus status)
        // {
        //     Debug.Log("SignStatus = " + status);
        //     if (status == SignInStatus.Success)
        //     {
        //         var userId = PlayGamesPlatform.Instance?.GetUserId();
        //         Debug.Log("UserId = " + userId);
        //
        //         SetId(userId);
        //     }
        //     else
        //     {
        //         PlayGamesPlatform.Instance?.RequestRecallAccess(
        //             (recallAccess) =>
        //             {
        //                 
        //             }
        //         );
        //     }
        //
        //     _endAuth = true;
        // }
#endif

        private void SocialAuthenticateCallback(bool success, string error)
        {
            if (success)
            {
                Debug.Log("Success Social Authenticate");
                Debug.Log("UserName = " + Social.localUser.userName);

                SetId(Social.localUser.id);

                // Debug.Log("SocialAuthenticateCallback = " + _id);
            }
            else
            {
                Debug.Log("error = " + error);
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

