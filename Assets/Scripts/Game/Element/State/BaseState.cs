using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Element.State
{
    public class BaseState
    {
        private static BaseState _instance = null;

        public static BaseState Create()
        {
            _instance = new BaseState();
            
            return _instance?.Initialize();
        }

        public static BaseState Create(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            _instance = new BaseState();
            
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

        public bool CheckEqual(BaseState state)
        {
            return Type.Equals(state.Type);
        }

        public virtual BaseState Initialize(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            return this;
        }

        public virtual BaseState Initialize()
        { 
            return this;
        }

        public virtual void Touch(Touch touch)
        {

        }

        public virtual void Apply(BaseElement gameBaseElement)
        {
            _gameBaseElement = gameBaseElement;
        }

        public virtual void End()
        {

        }

        public virtual void ChainUpdate()
        {

        }
    }
}
