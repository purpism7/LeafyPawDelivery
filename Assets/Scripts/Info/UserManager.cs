using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

//using Firebase.Database;
using Cysharp.Threading.Tasks;

using GameSystem;

namespace Info
{
    public class UserManager : Singleton<UserManager>
    {
        public static bool _isFirst = true;
        public static bool IsFirst
        {
            get
            {
                if (!_isFirst)
                    return false;

                if(System.Boolean.TryParse(PlayerPrefs.GetString("IsFirst"), out bool isFirst))
                {
                    _isFirst = isFirst;

                    return isFirst;
                }

                return true;
            }
            private set
            {
                if (!_isFirst)
                    return;

                _isFirst = value;

                PlayerPrefs.SetString("IsFirst", value.ToString());
            }
        }

        private string _jsonFilePath = string.Empty;
        private const string _fileName = "User.txt";
        private const string _scretKey = "hANkyulusEr";

        private User _user = null;

        public User User { get { return _user; } }

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
            try
            {
                var path = Utility.GetInfoPath();
                _jsonFilePath = Path.Combine(path, _fileName);
                //Debug.Log("User _jsonFilePath = " + _jsonFilePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string decodeStr = string.Empty;

                // 재 설치 시, 클라우드에 저장된 데이터가 있는지 체크 후 가져오기.
                if (Auth.EGameType_ == Auth.EGameType.New)
                {
                    CreateUserInfo();

                    yield break;
                }
                else if (Auth.EGameType_ == Auth.EGameType.Continue)
                {
                    decodeStr = Plugin.Native.Instance.SaveValue;

                    //Debug.Log("decodeStr = " + decodeStr);
                    if (string.IsNullOrEmpty(decodeStr))
                    {
                        CreateUserInfo();

                        yield break;
                    }
                }
                else
                {
                    if (!System.IO.File.Exists(_jsonFilePath))
                    {
                        CreateUserInfo();

                        yield break;
                    }

                    decodeStr = System.IO.File.ReadAllText(_jsonFilePath);
                }

                var jsonStr = decodeStr.Decrypt(_scretKey);

                _user = JsonUtility.FromJson<Info.User>(jsonStr);
                //Debug.Log("User = " + jsonStr);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 이미 UnauthorizedAccessException이 발생한 경우에는 다시 던지지 않고 예외를 처리합니다.
                Debug.LogError("Unauthorized access: " + ex.Message);
            }
            catch (IOException ex)
            {
                // IOException을 UnauthorizedAccessException으로 다시 던집니다.
                throw new UnauthorizedAccessException("Access to the path is unauthorized.", ex);
            }

            
            //string jsonStr = Plugin.Native.Instance?.GetString(GameSystem.Auth.ID);
            //if(string.IsNullOrEmpty(jsonStr))
            //{
            //    CreateUserInfo();

            //    yield break;
            //}
            //var fullPath = Path.Combine(path, _fileName);
            //Debug.Log("fullPath = " + fullPath);
            //Debug.Log("Application.persistentDataPath = " + Application.persistentDataPath);
            //var fullPath = _userInfoJsonFilePath + _fileName;

            //FileStream fileStream = File.Open(_jsonFilePath, FileMode.Open);

            //if (System.IO.File.Exists(_jsonFilePath))
            //{
            //    try
            //    {
            //        var jsonStr = System.IO.File.ReadAllText(_jsonFilePath);
            //        User = JsonUtility.FromJson<Info.User>(jsonStr);
            //        Debug.Log(jsonStr);
            //        //System.Text.Encoding.ASCII.GetBytes(jsonStr);


            //    }
            //    catch (UnauthorizedAccessException ex)
            //    {
            //        // 이미 UnauthorizedAccessException이 발생한 경우에는 다시 던지지 않고 예외를 처리합니다.
            //        Debug.LogError("Unauthorized access: " + ex.Message);
            //    }
            //    catch (IOException ex)
            //    {
            //        // IOException을 UnauthorizedAccessException으로 다시 던집니다.
            //        throw new UnauthorizedAccessException("Access to the path is unauthorized.", ex);
            //    }
            //}
            //else
            //{
            //    CreateUserInfo();
            //}

            //var jsonStr = decryptJsonStr.Decrypt("leafypawdelivery");


            //Plugin.Native.Instance?.LoadUserJson(GameSystem.Auth.ID);

            //CreateUserInfo();
            //Save


            //yield return null;
        }

        private void CreateUserInfo()
        {
            if (_user != null)
                return;

            _user = new Info.User();

            var placeId = Game.Data.Const.StartPlaceId;
            var currency = Game.Data.Const.GetStartCurrency(placeId);

            SaveCurrency(currency);
        }

        public void AddAnimal(Info.Animal animalInfo)
        {
            _user?.AddAnimal(animalInfo);

            Save();
        }

        public void AddAnimalSkin(int id, int skinId)
        {
            _user?.AddAnimalSkin(id, skinId);

            Save();
        }

        public void AddObject(int id)
        {
            _user?.AddObject(id);

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

        //var placeId = Game.Data.Const.StartPlaceId;
        //var currency = Game.Data.Const.GetStartCurrency(placeId);
        //SaveCurrency(currency);
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
            if (_user == null)
                return;

            SaveAsync().Forget();

            var jsonStr = JsonUtility.ToJson(_user);
            var encodeStr = jsonStr.Encrypt(_scretKey);
            //var bytes = System.Text.Encoding.UTF8.GetBytes(jsonStr);
            //var encodeStr = System.Convert.ToBase64String(bytes);

            System.IO.File.WriteAllText(_jsonFilePath, encodeStr);
            //Debug.Log("SaveAsync jsonStr = " + jsonStr);
            if (!string.IsNullOrEmpty(GameSystem.Auth.ID))
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    Plugin.Native.Instance?.SetString(typeof(User).Name, encodeStr);
                }
                else
                {
                    Plugin.Native.Instance?.SetString(GameSystem.Auth.ID, encodeStr);
                }
            }

            if (IsFirst)
            {
                IsFirst = false;
            }
        }

        private async UniTask SaveAsync()
        {
            try
            {
                var worldTime = GameSystem.WorldTime.Get;
                DateTime? saveDateTime = null;
                if (worldTime != null)
                {
                    saveDateTime = await worldTime.RequestAsync();
                }

                _user?.UpdateDateTime(saveDateTime);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public bool SaveLastPlaceId()
        {
            if (_user == null)
                return false;

            if (Game.Data.Const.LastPlaceId <= User.LastPlaceId)
                return false;

            _user.SetLastPlaceId();

            Save();

            return true;
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

        public void SetCurrency(Game.Type.EElement eElement, int currency)
        {
            _user?.SetCurrency(eElement, currency);
        }

        public void SetCurrency(User.Currency currency)
        {
            _user?.SetCurrency(currency);
        }

        public void SaveCurrency(User.Currency currency)
        {
            _user?.SetCurrency(currency);

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
            _user?.SetStory(GameUtils.ActivityPlaceId, storyId);

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
    }
}


