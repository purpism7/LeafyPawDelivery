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

        public abstract void Init(params object[] objs);
        public virtual void Init<T>(T tData) where T : BaseData
        {

        }

        public abstract void ChainUpdate();
    }
}

