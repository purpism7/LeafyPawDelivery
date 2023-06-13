using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Game.Manager
{
    public class BaseData
    {

    }

    public abstract class Base<T>: MonoBehaviour where T : BaseData
    {
        private void Awake()
        {
            Initialize();
        }

        protected abstract void Initialize();
        public abstract IEnumerator CoInit(T data);
    }
}

