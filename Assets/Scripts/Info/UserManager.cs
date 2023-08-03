using System.Collections;
using System.Collections.Generic;
using GameSystem.Firebase;
using UnityEngine;
using System.Linq;

namespace Info
{
    public class UserManager : Singleton<UserManager>
    {
        private const string KeyUserStoryList = "StoryList";

#if UNITY_EDITOR
        readonly private string _userInfoJsonFilePath = "Assets/Info/User.json";
#else
        readonly private string _userInfoJsonFilePath = Application.persistentDataPath + "/Info/User.json";
#endif
        
        public User User { get; private set; } = null;

        //private Dictionary<System.Type, Holder.Base> _holderDic = new();

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
                yield break;

            Debug.Log("firebase.Auth.UserId = " + firebaseMgr.Auth.UserId);
            yield return StartCoroutine(database?.CoLoad(firebaseMgr.Auth.UserId,
                (dataSnapshot) =>
                {
                    Debug.Log("dataSnapshot = " + dataSnapshot);
                    SetUserInfo(dataSnapshot);

                    endLoad = true;
                }));

            yield return new WaitUntil(() => endLoad);

            Debug.Log("End Load CoLoadUserInfo");
        }

        private void SetUserInfo(Firebase.Database.DataSnapshot dataSnapshot)
        {
            if (dataSnapshot == null)
                return;

            User = new Info.User();
            Debug.Log("SetUserInfo = " + dataSnapshot.Value);
            if (dataSnapshot.Value != null)
            {
                foreach (var data in dataSnapshot.Children)
                {
                    if (data == null)
                        continue;

                    Debug.Log("data = " + data);

                    switch(data.Key)
                    {
                        case "Cash":
                            {
                                User.Cash = (long)data.Value;

                                break;
                            }

                        case "CurrencyList":
                            {
                                SetCurrencyList(data);

                                break;
                            }

                        case KeyUserStoryList:
                            {
                                break;
                            }
                    }
                }
            }
            else
            {
                Debug.Log("No UserInfo");

                var placeId = Game.Data.Const.StartPlaceId;
                var currency = Game.Data.Const.GetStartCurrency(placeId);

                User.CurrencyList.Add(currency);

                var firebase = GameSystem.FirebaseManager.Instance;
                if (firebase != null)
                {
                    var database = firebase.Database;

                    database?.Save(firebase.Auth.UserId, JsonUtility.ToJson(User));
                }
            }
        }

        private void SetCurrencyList(Firebase.Database.DataSnapshot data)
        {
            if (data == null)
                return;

            User.CurrencyList.Clear();

            var currencyList = data.Value as IList;
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

                User.CurrencyList.Add(currency);
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

        public void SaveStoryList(int storyId)
        {
            var placeMgr = MainGameManager.Instance?.placeMgr;
            if (placeMgr == null)
                return;

            int placeId = placeMgr.ActivityPlaceId;

            User?.AddStory(placeId, storyId);

            var firebaseMgr = GameSystem.FirebaseManager.Instance;
            if (firebaseMgr == null)
                return;

            var userId = firebaseMgr.Auth?.UserId;
            if (string.IsNullOrEmpty(userId))
                return;

            firebaseMgr?.Database?.SaveChild(userId, KeyUserStoryList, JsonUtility.ToJson(User.StoryList));
        }
    }
}


