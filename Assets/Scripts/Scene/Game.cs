using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace Scene
{
    public class Game : Base, Preprocessing.IListener
    {
        [SerializeField] private Preprocessing _preprocessing = null;
        
        public override void Init(IListener iListener)
        {
            base.Init(iListener);
            
            _preprocessing?.Init(this);
        }

        void Preprocessing.IListener.End()
        {
            _iListener?.EndLoad();
        }
    }
}