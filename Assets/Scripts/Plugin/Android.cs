using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif

namespace Plugin
{
    public class Android : Base
    {
        public override void Initialize()
        {
            //string userInfoJsonFilePath = Application.persistentDataPath + "/Info/User.json";
            //if (System.IO.File.Exists(userInfoJsonFilePath))
            //{
            //    System.IO.File.Delete(userInfoJsonFilePath);
            //}
            //PlayGamesPlaform.Ins
            //PlayGamesClientConfiguration conf = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
            //PlayGamesPlatform.InitializeInstance(conf);
            //PlayGamesPlatform.DebugLogEnabled = true;
            //PlayGamesPlatform.Activate();
       
        }

#if UNITY_ANDROID
        private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                // handle reading or writing of saved game.
                //game

                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();

                savedGameClient?.CommitUpdate(game, update, null, OnSavedGameWritten);
            }
            else
            {
                // handle error
            }
        }

        private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                // handle reading or writing of saved game.
            }
            else
            {
                // handle error
            }
        }
#endif

        public override void SetString(string key, string value)
        {
#if UNITY_ANDROID
            var jsonFilePath = string.Format(_userInfoJsonFilePath, key);

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(jsonFilePath, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
#endif
        }

        public override string GetString(string key)
        {
            return string.Empty;
        }
    }
}

