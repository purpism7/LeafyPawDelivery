using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

namespace GameSystem
{
    public class Firestore
    {

        public IEnumerator CoInit()
        {
            var firestore = FirebaseFirestore.DefaultInstance;

            CollectionReference collectionRef = firestore.Collection("Users");

            Debug.Log(collectionRef.Id);

            yield return collectionRef.GetSnapshotAsync();


        }
    }
}

