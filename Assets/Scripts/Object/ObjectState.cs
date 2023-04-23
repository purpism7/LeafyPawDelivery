using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class ObjectState
    {
        protected Game.Object _object = null;

        public System.Type Type
        {
            get
            {
                return GetType();
            }
        }

        public abstract void Apply(Game.Object obj);
        public abstract void Touch(Touch touch);
    }
}
