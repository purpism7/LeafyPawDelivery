using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    public abstract class BaseCreator<T>
    {
        public abstract T Create();
        //public abstract void Create();
    }
}

