using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine.SocialPlatforms.GameCenter;

using GameSystem;

using Type = Game.Type;

namespace Scene
{
    public class Game : Base, Preprocessing.IListener
    {
        [SerializeField] private Preprocessing _preprocessing = null;

        //string Signature;
        //string TeamPlayerID;
        //string Salt;
        //string PublicKeyUrl;
        //ulong Timestamp;

        private void Start()
        {
            // GameScene 에서 바로 실행 시, 동작.
            // Loading 거쳐서 들어올 경우 에는, ActiveScene 이 LoadingScene 임.
            if (Enum.TryParse(SceneManager.GetActiveScene().name.Replace("Scene", ""), out Type.EScene eSceneType))
            {
                if (eSceneType == Type.EScene.Game)
                {
                    Info.Setting setting = new();
                    setting?.InitializeLocale();

                    _preprocessing?.Init(this);
                }
            }


            
            Debug.Log("Game Start");
            //await Login();
            //await SignInWithAppleGameCenterAsync(Signature, TeamPlayerID, PublicKeyUrl, Salt, Timestamp);
        }

        //public async Task Login()
        //{
        //    if (!GKLocalPlayer.Local.IsAuthenticated)
        //    {
        //        // Perform the authentication.
        //        var player = await GKLocalPlayer.Authenticate();
        //        Debug.Log($"GameKit Authentication: player {player}");

        //        // Grab the display name.
        //        var localPlayer = GKLocalPlayer.Local;
        //        Debug.Log($"Local Player: {localPlayer.DisplayName}");

        //        // Fetch the items.
        //        var fetchItemsResponse = await GKLocalPlayer.Local.FetchItems();

        //        Signature = Convert.ToBase64String(fetchItemsResponse.GetSignature());
        //        TeamPlayerID = localPlayer.TeamPlayerId;
        //        Debug.Log($"Team Player ID: {TeamPlayerID}");

        //        Salt = Convert.ToBase64String(fetchItemsResponse.GetSalt());
        //        PublicKeyUrl = fetchItemsResponse.PublicKeyUrl;
        //        Timestamp = fetchItemsResponse.Timestamp;

        //        Debug.Log($"GameKit Authentication: signature => {Signature}");
        //        Debug.Log($"GameKit Authentication: publickeyurl => {PublicKeyUrl}");
        //        Debug.Log($"GameKit Authentication: salt => {Salt}");
        //        Debug.Log($"GameKit Authentication: Timestamp => {Timestamp}");
        //    }
        //    else
        //    {
        //        Debug.Log("AppleGameCenter player already logged in.");
        //    }
        //}

        //private async Task SignInWithAppleGameCenterAsync(string signature, string teamPlayerId, string publicKeyURL, string salt, ulong timestamp)
        //{
        //    try
        //    {
        //        await AuthenticationService.Instance.SignInWithAppleGameCenterAsync(signature, teamPlayerId, publicKeyURL, salt, timestamp);
        //        Debug.Log("SignIn is successful.");
        //    }
        //    catch (AuthenticationException ex)
        //    {
        //        // Compare error code to AuthenticationErrorCodes
        //        // Notify the player with the proper error message
        //        Debug.LogException(ex);
        //    }
        //    catch (RequestFailedException ex)
        //    {
        //        // Compare error code to CommonErrorCodes
        //        // Notify the player with the proper error message
        //        Debug.LogException(ex);
        //    }
        //}



        public override void Init(IListener iListener)
        {
            base.Init(iListener);
            
            _preprocessing?.Init(this);
        }

        void Preprocessing.IListener.End()
        {
            _iListener?.EndLoad();
        }
    }
}