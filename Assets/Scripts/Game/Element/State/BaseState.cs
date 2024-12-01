using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Element.State
{
    public class Base
    {
        private static Base _instance = null;

        public static Base Create()
        {
            _instance = new Base();
            
            return _instance?.Initialize();
        }

        public static Base Create(GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            _instance = new Base();
            
            return _instance?.Initialize(gameCameraCtr, iGrid);
        }

        protected Game.BaseElement _gameBaseElement = null;

        public Game.Type.EElementState Type { get; protected set; } = Game.Type.EElementState.None;

        public bool CheckState(System.Type type)
        {
            return GetType() == type;
        }

        public virtual Base Initialize(GameSystem.GameCameraController gameCameraCtr = null, GameSystem.IGrid iGrid = null)
        {
            return this;
        }

        // public virtual BaseState Initialize()
        // { 
        //     return this;
        // }

        public virtual void Touch(TouchPhase touchPhase, Touch? touch)
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
