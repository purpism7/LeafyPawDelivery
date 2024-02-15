using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//using Firebase.Database;
using Cysharp.Threading.Tasks;

namespace Info
{
    public class UserManager : Singleton<UserManager>
    {
#if UNITY_EDITOR
        private string _userInfoJsonFilePath = "Assets/Info/User.json";
#else
        private string _userInfoJsonFilePath = Application.persistentDataPath + "/Info/User.json";
#endif

        public User User { get; private set; } = null;

        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                //Debug.Log("UserManager OnApplicationPause");
                Save();
                //Save();
            }
        }

        private void OnApplicationQuit()
        {
            Save();
            //Save();
        }

        protected override void Initialize()
        {
            
        }
        
        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());

            yield return StartCoroutine(CoLoadLocalUserInfo());
            //yield return StartCoroutine(CoLoadUserInfo());
        }

        private IEnumerator CoLoadLocalUserInfo()
        {
            //Debug.Log("iCloud = [" + GameSystem.Auth.ID + "] "+ iOSPlugin.iCloudGetStringValue(GameSystem.Auth.ID));

            if (!System.IO.File.Exists(_userInfoJsonFilePath))
            {
                CreateUserInfo();

                yield break;
            }
            //string jsonStr = Plugin.Native.Instance?.GetString(GameSystem.Auth.ID);
            //if(string.IsNullOrEmpty(jsonStr))
            //{
            //    CreateUserInfo();

            //    yield break;
            //}

            var jsonStr = System.IO.File.ReadAllText(_userInfoJsonFilePath);
            //var jsonStr = decryptJsonStr.Decrypt("leafypawdelivery");

            User = JsonUtility.FromJson<Info.User>(jsonStr);
            //Plugin.Native.Instance?.LoadUserJson(GameSystem.Auth.ID);

            CreateUserInfo();

            Debug.Log(jsonStr);
            
            yield return null;
        }

        private void CreateUserInfo()
        {
            if (User != null)
                return;

            User = new Info.User();

            //var jsonString = JsonUtility.ToJson(User);
            //System.IO.File.WriteAllText(_userInfoJsonFilePath, jsonString);

            
            Save();
        }

        //private IEnumerator CoLoadUserInfo()
        //{
        //    Debug.Log("CoLoadUserInfo()");
        //    var firebaseMgr = GameSystem.FirebaseManager.Instance;
        //    if (firebaseMgr == null)
        //        yield break;
            
        //    bool endLoad = false;

        //    var database = firebaseMgr.Database;
        //    if (database == null)
        //    {
        //        if (Application.isEditor)
        //        {
        //            User = new Info.User();
        //            //var placeId = Game.Data.Const.StartPlaceId;
        //            //var currency = Game.Data.Const.GetStartCurrency(placeId);
        //        }

        //        yield break;
        //    }

        //    yield return StartCoroutine(database?.CoLoad(firebaseMgr.Auth.UserId,
        //        (dataSnapshot) =>
        //        {
        //            SetUserInfo(dataSnapshot);

        //            endLoad = true;
        //        }));

        //    yield return new WaitUntil(() => endLoad);
        //}

        //private void SetUserInfo(Firebase.Database.DataSnapshot dataSnapshot)
        //{
        //    if (dataSnapshot == null)
        //        return;

        //    User = new Info.User();
        //    Debug.Log("SetUserInfo = " + dataSnapshot.Value);
        //    if (dataSnapshot.Value != null)
        //    {
        //        //const string KeyDatas = "Datas";

        //        foreach (var data in dataSnapshot.Children)
        //        {
        //            if (data == null)
        //                continue;

        //            switch(data.Key)
        //            {
        //                case "Cash":
        //                    {
        //                        User.Cash = (long)data.Value;

        //                        break;
        //                    }

        //                case KeyUserCurrency:
        //                    {
        //                        SetCurrency(data);

        //                        break;
        //                    }

        //                case KeyUserStory:
        //                    {
        //                        SetStory(data);

        //                        break;
        //                    }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("No UserInfo");

        //        var firebase = GameSystem.FirebaseManager.Instance;
        //        if (firebase == null)
        //            return;

        //        var database = firebase.Database;
        //        database?.Save(firebase.Auth.UserId, JsonUtility.ToJson(User));

        //        var placeId = Game.Data.Const.StartPlaceId;
        //        var currency = Game.Data.Const.GetStartCurrency(placeId);
        //        SaveCurrency(currency);
        //    }
        //}

        //private void SetCurrency(Firebase.Database.DataSnapshot dataSnapshot)
        //{
        //    if (dataSnapshot == null)
        //        return;

        //    User?.CurrencyList?.Clear();

        //    var currencyList = dataSnapshot.Value as IList;
        //    foreach (IDictionary currencyDic in currencyList)
        //    {
        //        var currency = new User.Currency();

        //        var enumerator = currencyDic.GetEnumerator();
        //        while (enumerator.MoveNext())
        //        {
        //            DictionaryEntry dicEntry = (DictionaryEntry)enumerator.Current;

        //            string key = dicEntry.Key.ToString();
                    
        //            if (key.Equals("Animal"))
        //            {
        //                currency.Animal = (long)dicEntry.Value;
        //            }
        //            else if (key.Equals("Object"))
        //            {
        //                currency.Object = (long)dicEntry.Value;
        //            }
        //            else if (key.Equals("PlaceId"))
        //            {
        //                var value = (long)dicEntry.Value;
        //                currency.PlaceId = (int)value;
        //            }
        //        }

        //        User?.CurrencyList?.Add(currency);
        //    }
        //}

        //private void SetStory(Firebase.Database.DataSnapshot dataSnapshot)
        //{
        //    if (dataSnapshot == null)
        //        return;

        //    User?.StoryList?.Clear();

        //    var storyList = dataSnapshot.Value as IList;
        //    foreach (IDictionary storyDic in storyList)
        //    {
        //        var story = new User.Story();

        //        var enumerator = storyDic.GetEnumerator();
        //        while (enumerator.MoveNext())
        //        {
        //            DictionaryEntry dicEntry = (DictionaryEntry)enumerator.Current;

        //            string key = dicEntry.Key.ToString();

        //            if (key.Equals("StoryId"))
        //            {
        //                var value = (long)dicEntry.Value;
        //                story.StoryId = (int)value;
        //            }
        //            else if (key.Equals("PlaceId"))
        //            {
        //                var value = (long)dicEntry.Value;
        //                story.PlaceId = (int)value;
        //            }
        //        }

        //        User?.StoryList?.Add(story);
        //    }
        //}

        private void Save()
        {
            var jsonStr = JsonUtility.ToJson(User);

            //Plugin.Native.Instance?.SetString(GameSystem.Auth.ID, jsonStr);
            System.IO.File.WriteAllText(_userInfoJsonFilePath, jsonStr);

            //iOSPlugin.iCloudSaveStringValue(GameSystem.Auth.ID, jsonString);
        }

        public void SaveLastPlaceId()
        {
            if (User == null)
                return;

            User.SetLastPlaceId(User.LastPlaceId + 1);

            Save();
        }        

        //private void Save()
        //{
        //    var firebaseMgr = GameSystem.FirebaseManager.Instance;
        //    if (firebaseMgr == null)
        //        return;

        //    var userId = firebaseMgr.Auth?.UserId;
        //    if (string.IsNullOrEmpty(userId))
        //        return;

        //    var jsonString = JsonUtility.ToJson(User);
        //    firebaseMgr?.Database?.Save(userId, jsonString);
        //}

        public void SaveCurrency(User.Currency currency)
        {
            User?.SetCurrency(currency);

            Save();

            //var firebaseMgr = GameSystem.FirebaseManager.Instance;
            //if (firebaseMgr == null)
            //    return;

            //var userId = firebaseMgr.Auth?.UserId;
            //if (string.IsNullOrEmpty(userId))
            //    return;

            //firebaseMgr?.Database?.SaveChild(userId, KeyUserCurrency, JsonUtility.ToJson(User.CurrencyList.ToArray()));
        }

        //public void SaveCash(long cash)
        //{
        //    User?.SetCash(cash);

        //    Save();

        //    //var firebaseMgr = GameSystem.FirebaseManager.Instance;
        //    //if (firebaseMgr == null)
        //    //    return;

        //    //var userId = firebaseMgr.Auth?.UserId;
        //    //if (string.IsNullOrEmpty(userId))
        //    //    return;

        //    //firebaseMgr?.Database?.SaveChild(userId, KeyUserCash, JsonUtility.ToJson(User.Cash));
        //}

        public void SaveStory(int storyId)
        {
            User?.SetStory(GameUtils.ActivityPlaceId, storyId);

            Save();

            //var firebaseMgr = GameSystem.FirebaseManager.Instance;
            //if (firebaseMgr == null)
            //    return;

            //var userId = firebaseMgr.Auth?.UserId;
            //if (string.IsNullOrEmpty(userId))
            //    return;

            //var jsonStr = JsonHelper.ToJson(User.StoryList.ToArray());
            //Debug.Log("StoryList json str = " + jsonStr);

            //firebaseMgr?.Database?.SaveChild(userId, KeyUserStory, jsonStr);

        }

        public void Reset()
        {
            Plugin.Native.Instance?.SetString(GameSystem.Auth.ID, string.Empty);

            
        }
    }
}


