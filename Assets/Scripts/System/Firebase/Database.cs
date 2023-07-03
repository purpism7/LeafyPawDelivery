using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Database;

namespace GameSystem.Firebase
{
    public class Database : MonoBehaviour
    {

        //public async void Init()
        //{
        //    var firestore = FirebaseFirestore.DefaultInstance;

        //    CollectionReference collectionRef = firestore.Collection("Users");
        //    var snapshot = await collectionRef.GetSnapshotAsync();

        //    foreach(var document in snapshot.Documents)
        //    {

        //    }


        //}

        public IEnumerator CoInit()
        {
            //var rootRef = FirebaseDatabase.DefaultInstance?.RootReference;

            
            ////yield return firestore.Collection("Users").Document("pChGv53CvkqUXHnoOdMQ").GetSnapshotAsync().ContinueWith(
            ////    task =>
            ////    {
            ////        Debug.Log("pChGv53CvkqUXHnoOdMQ = " + task.Result.ToDictionary()["Id"]);
            ////    });


            //var userInfo = new Info.User();
            //userInfo.CurrencyList.Add(new Info.User.Currency());
            //var jsonStr = JsonUtility.ToJson(userInfo);
            //Debug.Log(jsonStr);

            ////var reference = firestore.Collection("Users");

            //rootRef.Child("11").SetRawJsonValueAsync(jsonStr);
            ////yield return (reference.AddAsync(jsonStr).ContinueWith(
            ////    task =>
            ////    {
            //        Debug.Log(task.Result);
            ////    }));


            //var reference = firestore.Collection("Users");
            //yield return reference.GetSnapshotAsync().ContinueWith(
            //    task =>
            //    {
            //        var result = task.Result;

            //        //result.Documents
            //    });

            yield return null;
        }

        public IEnumerator CoLoad(string pathStr, System.Action<object> resAction)
        {
            var rootRef = FirebaseDatabase.DefaultInstance?.RootReference;
            if (rootRef == null)
            {
                resAction?.Invoke(null);

                yield break;
            }

            var databaseRef = rootRef.Child(pathStr);
            if (databaseRef == null)
            {
                resAction?.Invoke(null);

                yield break;
            }

            bool endLoad = false;

            yield return databaseRef.GetValueAsync().ContinueWith(
                task =>
                {
                    var result = task.Result.GetValue(true);

                    resAction?.Invoke(result);

                    endLoad = true;
                });

            yield return new WaitUntil(() => endLoad);

            Debug.Log("End Load database");
        }

        public IEnumerator CoSave(string pathStr, string jsonStr)
        {
            var rootRef = FirebaseDatabase.DefaultInstance?.RootReference;
            if (rootRef == null)
                yield break;

            var databaseRef = rootRef.Child(pathStr);

            databaseRef.SetRawJsonValueAsync(jsonStr);
        }
    }
}

