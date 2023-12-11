using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace FirebaseSystem
{
    public abstract class Base : MonoBehaviour
    {
        public abstract IEnumerator CoInit();
        public abstract UniTask AsyncInitialize();
    }
}

