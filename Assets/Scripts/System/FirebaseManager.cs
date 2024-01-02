using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Analytics;

namespace GameSystem
{
    public class FirebaseManager : Singleton<FirebaseManager>
    {
        public FirebaseSystem.Auth Auth { get; private set; } = null;
        public FirebaseSystem.Database Database { get; private set; } = null;

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

                    check = true;
                });

            yield return new WaitUntil(() => check);

            //Auth = gameObject.GetOrAddComponent<FirebaseSystem.Auth>();
            //Debug.Log("Auth = " + Auth.name);
            //yield return StartCoroutine(Auth.CoInit());

            //Database = gameObject.GetOrAddComponent<FirebaseSystem.Database>();
            //Debug.Log("Database = " + Database.name);
            //yield return StartCoroutine(Database.CoInit());

            ////yield return new WaitUntil(() => check);
            Debug.Log("End Init FirebaseManager");
        }
    }
}