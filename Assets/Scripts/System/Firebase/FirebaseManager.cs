using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Analytics;

namespace GameSystem
{
    public class FirebaseManager : Singleton<FirebaseManager>
    {
        public Firebase.Auth Auth { get; private set; } = null;
        public Firebase.Database Database { get; private set; } = null;

        private FirebaseApp _firebaseApp = null;

        protected override void Initialize()
        {
            //FirebaseApp.Create();
        }

        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());

            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

            bool check = false;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(
                (task) =>
                {
                    if (task.Result != DependencyStatus.Available)
                        return;

                    //Debug.Log("Firebase Check = " + task.Result);
                    //FirebaseApp.Create();

                    //FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    Debug.Log("CheckAndFix");
                    check = true;
                });

            yield return new WaitUntil(() => check);

            Auth = gameObject.GetOrAddComponent<Firebase.Auth>();
            Debug.Log("Auth = " + Auth.name);
            yield return StartCoroutine(Auth.CoInit());

            Database = gameObject.GetOrAddComponent<Firebase.Database>();
            Debug.Log("Database = " + Database.name);
            yield return StartCoroutine(Database.CoInit());

            //yield return new WaitUntil(() => check);
            Debug.Log("End Init FirebaseManager");
        }
    }
}