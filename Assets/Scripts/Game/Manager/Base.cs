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

    public class Base : MonoBehaviour
    {
       
    }

    public abstract class Base<T>: Base where T : BaseData
    {
        //private void Awake()
        //{
        //    Initialize();
        //}

        public virtual void ChainUpdate()
        {

        }

        public abstract MonoBehaviour Initialize();
        public abstract IEnumerator CoInitialize(T data);
    }
}

