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
        private System.Action<DataSnapshot> _dataSnapshotAction = null;


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

        public IEnumerator CoLoad(string pathStr, System.Action<DataSnapshot> resAction)
        {
            _dataSnapshotAction = resAction;

            var database = FirebaseDatabase.DefaultInstance;
            if (database == null)
            {
                _dataSnapshotAction?.Invoke(null);

                yield break;
            }

            var databaseRef = database.GetReference(pathStr);
            if (databaseRef == null)
            {
                _dataSnapshotAction?.Invoke(null);

                yield break;
            }

            bool endLoad = false;

            databaseRef.GetValueAsync().ContinueWith(
                (task) =>
                {
                    var result = task.Result;

                    _dataSnapshotAction.Invoke(result);
                  
                    Debug.Log("database value async");

                    endLoad = true;
                });

            yield return new WaitUntil(() => endLoad);

            Debug.Log("End Load database");
        }

        public void Save(string pathStr, string jsonStr)
        {
            var database = FirebaseDatabase.DefaultInstance;
            if (database == null)
                return;

            var databaseRef = database.GetReference(pathStr);
            Debug.Log("jsonStr = " + jsonStr);
            databaseRef.SetRawJsonValueAsync(jsonStr);
        }

        public void SaveChild(string pathStr, string child, string jsonStr)
        {
            var database = FirebaseDatabase.DefaultInstance;
            if (database == null)
                return;

            var databaseRef = database.GetReference(pathStr);
            var childDatabaseRef = databaseRef.Child(child);
            if(childDatabaseRef == null)
            {
                childDatabaseRef.Push();
            }

            childDatabaseRef.SetRawJsonValueAsync(jsonStr);
        }
    }
}

