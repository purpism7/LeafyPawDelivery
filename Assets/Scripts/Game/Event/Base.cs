using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Event
{
    public abstract class Base
    {
        public abstract void Initialize();
        public abstract void Starter(System.Action endAction);
        public abstract void Emit<T>(T t) where T : BaseData;
    }
}

