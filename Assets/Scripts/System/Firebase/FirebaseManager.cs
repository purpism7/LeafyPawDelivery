using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public class FirebaseManager
    {
        private static FirebaseManager _instance = null;

        public static FirebaseManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new();
                }

                return _instance;
            }
        }


        public Firestore Firestore { get; private set; } = null;

        public void Init()
        {
            Firestore = new();
        }
    }
}

