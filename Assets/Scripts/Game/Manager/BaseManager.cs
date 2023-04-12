using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Manager
{
    public class BaseData
    {

    }

    public abstract class Base<T>: MonoBehaviour where T : BaseData
    {
        public abstract IEnumerator CoInit(T data);
    }
}

