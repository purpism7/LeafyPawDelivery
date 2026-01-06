using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class BaseElement : Base
    {
        
    }

    public abstract class BaseElement<T> : BaseElement
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

        public abstract void Remove(int id, int uId);
        public abstract bool CheckExist(int id);
    }
}
