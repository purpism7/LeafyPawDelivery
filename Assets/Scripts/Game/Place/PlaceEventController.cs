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

        public void Initialize(IPlace iPlace, int id)
        {
            _dropCurrency = GetOrAdd<PlaceEvent.DropCurrency>(iPlace, id);
            _speechBubble = GetOrAdd<PlaceEvent.SpeechBubble>(iPlace, id);
            _dropItem = GetOrAdd<PlaceEvent.DropItem>(iPlace, id);
            _hiddenObject = GetOrAdd<PlaceEvent.HiddenObject>(iPlace, id);
        }

        private T GetOrAdd<T>(IPlace iPlace, int id) where T : PlaceEvent.Base
        {
            var placeGameObj = (iPlace as Place)?.gameObject;
            if (!placeGameObj)
                return default(T);

            return placeGameObj.GetOrAddComponent<T>()?.Initialize(iPlace, this, id) as T;
        }

        public void Start()
        {
            _dropCurrency?.StartDrop();
            _speechBubble?.Activate();
            _dropItem?.StartDrop();
        }

        public void End()
        {
            _dropCurrency?.StopDrop();
            _speechBubble?.Deactivate();
            _dropItem?.StopDrop();
        }

        void PlaceEvent.Base.IListener.Action(PlaceEvent.BaseData data)
        {
            Event?.Invoke(data);
        }
    }
}

