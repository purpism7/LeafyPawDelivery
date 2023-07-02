using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Database;

namespace GameSystem.Firebase
{
    public class Firestore : MonoBehaviour
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
            var rootRef = FirebaseDatabase.DefaultInstance?.RootReference;

            
            //yield return firestore.Collection("Users").Document("pChGv53CvkqUXHnoOdMQ").GetSnapshotAsync().ContinueWith(
            //    task =>
            //    {
            //        Debug.Log("pChGv53CvkqUXHnoOdMQ = " + task.Result.ToDictionary()["Id"]);
            //    });


            var userInfo = new Info.User();
            userInfo.CurrencyList.Add(new Info.User.Currency());
            var jsonStr = JsonUtility.ToJson(userInfo);
            Debug.Log(jsonStr);

            //var reference = firestore.Collection("Users");

            rootRef.Child("11").SetRawJsonValueAsync(jsonStr);
            //yield return (reference.AddAsync(jsonStr).ContinueWith(
            //    task =>
            //    {
            //        Debug.Log(task.Result);
            //    }));


            //var reference = firestore.Collection("Users");
            //yield return reference.GetSnapshotAsync().ContinueWith(
            //    task =>
            //    {
            //        var result = task.Result;

            //        //result.Documents
            //    });

            yield return null;
        }
    }
}

