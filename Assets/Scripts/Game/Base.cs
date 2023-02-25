using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class Base : MonoBehaviour
    {
        public int Id;

        public virtual void OnTouch()
        {

        }

        public abstract void Init(params object[] objs);
        public abstract void ChainUpdate();
    }
}

