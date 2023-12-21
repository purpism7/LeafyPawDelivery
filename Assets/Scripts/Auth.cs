using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Unity.Services.Authentication;
using Unity.Services.Core;

namespace GameSystem
{
    public class Auth
    {
        private static Auth _instance = null;
        public static string ID { get { return _instance?._id; } }

        private string _id = string.Empty;

        public Auth()
        {
            _instance = this;
        }

        public IEnumerator CoInitialize()
        {
            yield return UnityServices.InitializeAsync();
            Debug.Log(UnityServices.State);

            yield return AuthenticationService.Instance?.SignInAnonymouslyAsync();

            _id = AuthenticationService.Instance?.PlayerId;
            Debug.Log($"PlayerID: {_id}");

            Social.localUser.Authenticate(
               (success) =>
               {
                   Debug.Log("Success Social Authenticate");
                    //GameCenterPlatform.Show(true);
                    //Social.
                   Debug.Log(Social.localUser.id);
                   Debug.Log("UserName = " + Social.localUser.userName);
               });
        }
    }
}

