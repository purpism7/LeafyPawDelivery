using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public abstract class Base : MonoBehaviour
    {
        protected IPlace _iPlace = null;

        public virtual Base Initialize(IPlace iPlace)
        {
            _iPlace = iPlace;

            return this;
        }
    }
}

