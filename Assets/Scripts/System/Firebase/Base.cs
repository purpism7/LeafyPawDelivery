using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Firebase
{
    public abstract class Base : MonoBehaviour
    {
        public abstract IEnumerator CoInit();
    }
}

