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

        public abstract bool CheckEditObject { get; }
    }
}

