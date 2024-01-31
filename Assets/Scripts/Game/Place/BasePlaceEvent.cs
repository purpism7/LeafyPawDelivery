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
        protected int _placeId = 0;

        public virtual Base Initialize(IPlace iPlace, IListener iListener, int placeId)
        {
            _iPlace = iPlace;
            _iListener = iListener;
            _placeId = placeId;

            return this;
        }
    }
}

