using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public abstract class Base : MonoBehaviour
    {
        public interface IListener
        {
            void Action(PlaceEvent.BaseData data);
        }

        protected IPlace _iPlace = null;
        protected IListener _iListener = null;

        public virtual Base Initialize(IPlace iPlace, IListener iListener)
        {
            _iPlace = iPlace;
            _iListener = iListener;

            return this;
        }
    }
}

