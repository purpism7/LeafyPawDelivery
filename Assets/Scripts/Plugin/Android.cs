using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



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
        private void Save(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                ISavedGameClient iSavedGameClient = PlayGamesPlatform.Instance.SavedGame;
                SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();

                iSavedGameClient?.CommitUpdate(game, update, null,
                    (SavedGameRequestStatus status, ISavedGameMetadata game) =>
                    {
                        if (status == SavedGameRequestStatus.Success)
                        {
                            // handle reading or writing of saved game.
                        }
                        else
                        {
                            // handle error
                        }
                    });
            }
            else
            {
                // handle error
            }
        }

        private void Load(SavedGameRequestStatus status, ISavedGameMetadata game)
        {
            
        }
#endif

        public override void SetString(string key, string value)
        {
#if UNITY_ANDROID

            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(key, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood,
                (SavedGameRequestStatus status, ISavedGameMetadata game) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        ISavedGameClient iSavedGameClient = PlayGamesPlatform.Instance.SavedGame;
                        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();

                        iSavedGameClient?.CommitUpdate(game, update, System.Text.Encoding.UTF8.GetBytes(value),
                            (SavedGameRequestStatus status, ISavedGameMetadata game) =>
                            {
                                if (status == SavedGameRequestStatus.Success)
                                {
                                    // handle reading or writing of saved game.
                                }
                                else
                                {
                                    // handle error
                                }
                            });
                    }
                    else
                    {
                        // handle error
                    }
                });
#endif
        }

        public override void GetString(string key, System.Action<bool, string> endAction)
        {
#if UNITY_ANDROID
            if(!PlayGamesPlatform.Instance.IsAuthenticated())
                endAction?.Invoke(false, string.Empty);
            
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance?.SavedGame;
            savedGameClient?.OpenWithAutomaticConflictResolution(key, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood,
                (SavedGameRequestStatus status, ISavedGameMetadata game) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        ISavedGameClient iSavedGameClient = PlayGamesPlatform.Instance.SavedGame;

                        iSavedGameClient?.ReadBinaryData(game,
                            (SavedGameRequestStatus status, byte[] bytes) =>
                            {
                                if (status == SavedGameRequestStatus.Success)
                                {
                                    // handle reading or writing of saved game.
                                    SaveValue = System.Text.Encoding.UTF8.GetString(bytes);
                                    endAction?.Invoke(true, SaveValue);
                                }
                                else
                                {
                                    endAction?.Invoke(false, string.Empty);
                                    // handle error
                                }
                            });
                    }
                    else
                    {
                        // handle error
                    }
                });
#endif
        }
    }
}

