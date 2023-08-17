using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class BaseState<T> where T : Game.BaseElement
    {
        protected Game.BaseElement _gameBaseElement = null;

        public System.Type Type
        {
            get
            {
                return GetType();
            }
        }

        public abstract void Apply(T t);
        public abstract void Touch(Touch touch);
    }
}
