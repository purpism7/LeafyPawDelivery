using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;

namespace GameSystem
{
    public class FirebaseManager : Singleton<FirebaseManager>
    {
        public Firebase.Auth Auth { get; private set; } = null;
        public Firebase.Database Database { get; private set; } = null;

        protected override void Initialize()
        {
            FirebaseApp.Create();
        }

        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());

            Auth = gameObject.GetOrAddComponent<Firebase.Auth>();
            yield return StartCoroutine(Auth?.CoInit());

            Database = gameObject.GetOrAddComponent<Firebase.Database>();
            yield return StartCoroutine(Database?.CoInit());
        }
    }
}