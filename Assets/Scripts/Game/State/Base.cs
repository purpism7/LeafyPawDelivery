using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public abstract class Base
    {
        public System.Type Type
        {
            get
            {
                return GetType();
            }
        }

        public bool CheckState<T>() where T : Base
        {
            return Type.Equals(typeof(T));
        }

        public abstract void Initialize(MainGameManager mainGameMgr);
    }
}

