using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace Scene
{
    public abstract class Base : MonoBehaviour
    {
        public interface IListener
        {
            void EndLoad();
        }
        
        protected IListener _iListener = null;

        public virtual void Init(IListener iListener)
        {
            _iListener = iListener;
        }
    }
}

