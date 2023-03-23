using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BaseData
    {

    }

    public abstract class Base : MonoBehaviour
    {
        public int Id;

        public virtual void OnTouch()
        {

        }

        public abstract void ChainUpdate();
    }

    public abstract class Base<T> : Base where T : BaseData
    {
        protected T _data = default(T);

        public virtual void Init(T data)
        {
            _data = data;
        }

        public virtual IEnumerator CoInit(T data)
        {
            _data = data;

            yield return null;
        }
    }
}

