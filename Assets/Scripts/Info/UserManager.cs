using System.Collections;
using System.Collections.Generic;
using GameSystem.Firebase;
using UnityEngine;
using System.Linq;
using Firebase.Database;

namespace Info
{
    public class UserManager : Singleton<UserManager>
    {
        private const string KeyUserCurrency = "CurrencyList";
        private const string KeyUserCash = "Cash";
        private const string KeyUserStory = "StoryList";

#if UNITY_EDITOR
        readonly private string _userInfoJsonFilePath = "Assets/Info/User.json";
#else
        readonly private string _userInfoJsonFilePath = Application.persistentDataPath + "/Info/User.json";
#endif
        
        public User User { get; private set; } = null;

        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                Debug.Log("UserManager OnApplicationPause");
                Save();
            }
        }

        protected override void Initialize()
        {
            
        }
        
        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());
            yield return StartCoroutine(CoLoadUserInfo());
            yield return null;
        }

        private IEnumerator CoLoadUserInfo()
        {
            var firebaseMgr = GameSystem.FirebaseManager.Instance;
            if (firebaseMgr == null)
                yield break;
            
            bool endLoad = false;

            var database = firebaseMgr.Database;
            if (database == null)
            {
                if (Application.isEditor)
                {
                    User = new Info.User();
                    var placeId = Game.Data.Const.StartPlaceId;
                    var currency = Game.Data.Const.GetStartCurrency(placeId);
                }

                yield break;
            }

            yield return StartCoroutine(database?.CoLoad(firebaseMgr.Auth.UserId,
                (dataSnapshot) =>
                {
                    SetUserInfo(dataSnapshot);

                    endLoad = true;
                }));

            yield return new WaitUntil(() => endLoad);
        }

        private void SetUserInfo(Firebase.Database.DataSnapshot dataSnapshot)
        {
            if (dataSnapshot == null)
                return;

            User = new Info.User();
            Debug.Log("SetUserInfo = " + dataSnapshot.Value);
            if (dataSnapshot.Value != null)
            {
                //const string KeyDatas = "Datas";

                foreach (var data in dataSnapshot.Children)
                {
                    if (data == null)
                        continue;

                    switch(data.Key)
                    {
                        case "Cash":
                            {
                                User.Cash = (long)data.Value;

                                break;
                            }

                        case KeyUserCurrency:
                            {
                                SetCurrency(data);

                                break;
                            }

                        case KeyUserStory:
                            {
                                SetStory(data);

                                break;
                            }
                    }
                }
            }
            else
            {
                Debug.Log("No UserInfo");

                var firebase = GameSystem.FirebaseManager.Instance;
                if (firebase == null)
                    return;

                var database = firebase.Database;
                database?.Save(firebase.Auth.UserId, JsonUtility.ToJson(User));

                var placeId = Game.Data.Const.StartPlaceId;
                var currency = Game.Data.Const.GetStartCurrency(placeId);
                SaveCurrency(currency);
            }
        }

        private void SetCurrency(Firebase.Database.DataSnapshot dataSnapshot)
        {
            if (dataSnapshot == null)
                return;

            User?.CurrencyList?.Clear();

            var currencyList = dataSnapshot.Value as IList;
            foreach (IDictionary currencyDic in currencyList)
            {
                var currency = new User.Currency();

                var enumerator = currencyDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DictionaryEntry dicEntry = (DictionaryEntry)enumerator.Current;

                    string key = dicEntry.Key.ToString();
                    
                    if (key.Equals("Animal"))
                    {
                        currency.Animal = (long)dicEntry.Value;
                    }
                    else if (key.Equals("Object"))
                    {
                        currency.Object = (long)dicEntry.Value;
                    }
                    else if (key.Equals("PlaceId"))
                    {
                        var value = (long)dicEntry.Value;
                        currency.PlaceId = (int)value;
                    }
                }

                User?.CurrencyList?.Add(currency);
            }
        }

        private void SetStory(Firebase.Database.DataSnapshot dataSnapshot)
        {
            if (dataSnapshot == null)
                return;

            User?.StoryList?.Clear();

            var storyList = dataSnapshot.Value as IList;
            foreach (IDictionary storyDic in storyList)
            {
                var story = new User.Story();

                var enumerator = storyDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DictionaryEntry dicEntry = (DictionaryEntry)enumerator.Current;

                    string key = dicEntry.Key.ToString();

                    if (key.Equals("StoryId"))
                    {
                        var value = (long)dicEntry.Value;
                        story.StoryId = (int)value;
                    }
                    else if (key.Equals("PlaceId"))
                    {
                        var value = (long)dicEntry.Value;
                        story.PlaceId = (int)value;
                    }
                }

                User?.StoryList?.Add(story);
            }
        }

        public int GetLastStoryId(int placeId)
        {
            var story = User?.GetStory(placeId);
            if (story == null)
                return 0;

            return story.StoryId;
        }

        public void Save()
        {
            var firebaseMgr = GameSystem.FirebaseManager.Instance;
            if (firebaseMgr == null)
                return;

            var userId = firebaseMgr.Auth?.UserId;
            if (string.IsNullOrEmpty(userId))
                return;

            firebaseMgr?.Database?.Save(userId, JsonUtility.ToJson(User));
        }

        public void SaveCurrency(User.Currency currency)
        {
            User?.SetCurrency(currency);

            var firebaseMgr = GameSystem.FirebaseManager.Instance;
            if (firebaseMgr == null)
                return;

            var userId = firebaseMgr.Auth?.UserId;
            if (string.IsNullOrEmpty(userId))
                return;

            firebaseMgr?.Database?.SaveChild(userId, KeyUserCurrency, JsonUtility.ToJson(User.CurrencyList.ToArray()));
        }

        public void SaveCash(long cash)
        {
            User?.SetCash(cash);

            var firebaseMgr = GameSystem.FirebaseManager.Instance;
            if (firebaseMgr == null)
                return;

            var userId = firebaseMgr.Auth?.UserId;
            if (string.IsNullOrEmpty(userId))
                return;

            firebaseMgr?.Database?.SaveChild(userId, KeyUserCash, JsonUtility.ToJson(User.Cash));
        }

        public void SaveStory(int storyId)
        {
            var placeMgr = MainGameManager.Instance?.placeMgr;
            if (placeMgr == null)
                return;

            int placeId = placeMgr.ActivityPlaceId;

            User?.SetStory(placeId, storyId);

            var firebaseMgr = GameSystem.FirebaseManager.Instance;
            if (firebaseMgr == null)
                return;

            var userId = firebaseMgr.Auth?.UserId;
            if (string.IsNullOrEmpty(userId))
                return;

            var jsonStr = JsonHelper.ToJson(User.StoryList.ToArray());
            Debug.Log("StoryList json str = " + jsonStr);

            firebaseMgr?.Database?.SaveChild(userId, KeyUserStory, jsonStr);
        }
    }
}


