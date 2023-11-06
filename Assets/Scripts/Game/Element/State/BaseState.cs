using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Element.State
{
    public class BaseState<T> where T : Game.BaseElement
    {
        private static BaseState<T> _instance = null;

        public static BaseState<T> Create()
        {
            _instance = new BaseState<T>();

            return _instance?.Initialize();
        }

        public static BaseState<T> Create(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            _instance = new BaseState<T>();

            return _instance?.Initialize(gameCameraCtr, iGrid);
        }

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

        protected virtual BaseState<T> Initialize(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            return this;
        }

        protected virtual BaseState<T> Initialize()
        { 
            return this;
        }

        public virtual void Touch(Touch touch)
        {

        }

        public virtual void Apply(T t)
        {
            _gameBaseElement = t;
        }
    }
}
