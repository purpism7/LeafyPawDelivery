using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class PlaceEventController : PlaceEvent.Base.IListener
    {
        public static UnityEvent<PlaceEvent.BaseData> Event = new();

        private PlaceEvent.DropCurrency _dropCurrency = null;
        private PlaceEvent.SpeechBubble _speechBubble = null;
        private PlaceEvent.DropItem _dropItem = null;
        private PlaceEvent.HiddenObject _hiddenObject = null;

        public void Initialize(Place place, int id)
        {
            _dropCurrency = GetOrAdd<PlaceEvent.DropCurrency>(place, id);
            _speechBubble = GetOrAdd<PlaceEvent.SpeechBubble>(place, id);
            _dropItem = GetOrAdd<PlaceEvent.DropItem>(place, id);
            _hiddenObject = GetOrAdd<PlaceEvent.HiddenObject>(place, id);
        }

        private T GetOrAdd<T>(Place place, int id) where T : PlaceEvent.Base
        {
            var placeGameObj = place?.gameObject;
            if (!placeGameObj)
                return default(T);

            return placeGameObj.GetOrAddComponent<T>()?.Initialize(place, this, id) as T;
        }

        public void Start()
        {
            _dropCurrency?.StartDrop();
            _speechBubble?.Activate();
            _dropItem?.StartDrop();
            _hiddenObject?.Activate();
        }

        public void End()
        {
            _dropCurrency?.StopDrop();
            _speechBubble?.Deactivate();
            _dropItem?.StopDrop();
            _hiddenObject?.Deactivate();
        }

        void PlaceEvent.Base.IListener.Action(PlaceEvent.BaseData data)
        {
            Event?.Invoke(data);
        }
    }
}

