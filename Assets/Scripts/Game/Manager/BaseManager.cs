using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Manager
{
    public abstract class Base : MonoBehaviour
    {
        public abstract IEnumerator CoInit();
    }
}

