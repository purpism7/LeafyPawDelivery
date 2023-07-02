using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class FirebaseManager : Singleton<FirebaseManager>
    {
        public Firebase.Auth Auth { get; private set; } = null;
        public Firebase.Firestore Firestore { get; private set; } = null;

        protected override void Initialize()
        {
            
        }

        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());

            Auth = gameObject.GetOrAddComponent<Firebase.Auth>();
            yield return StartCoroutine(Auth?.CoInit());

            Firestore = gameObject.GetOrAddComponent<Firebase.Firestore>();
            yield return StartCoroutine(Firestore?.CoInit());
        }
    }
}