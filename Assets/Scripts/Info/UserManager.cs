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
                        User.CurrencyList.Clear();

                        var currencyList = data.Value as IList;
                        foreach(IDictionary currencyDic in currencyList)
                        {
                            //foreach(vadf wvr currency in currencyDic)
                            //{
                            //    Debug.Log(currency);
                            //}
                        }
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
    }
}


