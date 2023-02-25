using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Data
    {

    }

    public class Base : MonoBehaviour
    {

    }

    public abstract class Base<T> : Base where T : Data
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

