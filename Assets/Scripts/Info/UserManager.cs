using System.Collections;
using System.Collections.Generic;
using GameSystem.Firebase;
using UnityEngine;
using System.Linq;

namespace Info
{
    public class UserManager : Singleton<UserManager>
    {
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

            Debug.Log("CoLoadUserInfo");
            yield return StartCoroutine(CoLoadUserInfo());

            yield return null;
        }

        private IEnumerator CoLoadUserInfo()
        {
            var firebase = GameSystem.FirebaseManager.Instance;
            if (firebase == null)
                yield break;

            bool endLoad = false;

            var database = firebase.Database;
            if (database == null)
                yield break;

            Debug.Log("firebase.Auth.UserId = " + firebase.Auth.UserId);
            yield return StartCoroutine(database?.CoLoad(firebase.Auth.UserId,
                (dataSnapshot) =>
                {
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

                    if (data.Key.Equals("Cash"))
                    {
                        User.Cash = (long)data.Value;
                    }
                    else if (data.Key.Equals("CurrencyList"))
                    {
                        SetCurrencyList(data);
                    }
                }
            }
            else
            {
                Debug.Log("No UserInfo");

                User.CurrencyList.Add(
                    new User.Currency()
                    {
                        PlaceId = Game.Data.Const.StartPlaceId,
                    });

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
            var storyList = User?.StoryList;
            if (storyList == null)
                return 0;

            var index = placeId - 1;
            if (storyList.Count < index)
                return 1;

            return storyList[index];
        }
    }
}


