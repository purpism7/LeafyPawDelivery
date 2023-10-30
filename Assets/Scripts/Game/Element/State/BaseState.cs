using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Element.State
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

        public bool CheckEqual(BaseState<T> state)
        {
            return Type.Equals(state.Type);
        }

        public abstract BaseState<T> Create(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid);
        public abstract void Apply(T t);
        public abstract void Touch(Touch touch);
    }
}
