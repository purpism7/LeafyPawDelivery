using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class FirebaseManager : Singleton<FirebaseManager>
    {
        public Firestore Firestore { get; private set; } = new();

        protected override void Initialize()
        {
            
        }

        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());

            yield return Firestore?.CoInit();
        }
    }
}