using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlaceEventController
    {
        private PlaceEvent.DropCurrency _dropCurrency = null;
        private PlaceEvent.SpeechBubble _speechBubble = null;

        public void Initialize(IPlace iPlace)
        {
            _dropCurrency = GetOrAdd<PlaceEvent.DropCurrency>(iPlace);
            _speechBubble = GetOrAdd<PlaceEvent.SpeechBubble>(iPlace);
        }

        private T GetOrAdd<T>(IPlace iPlace) where T : PlaceEvent.Base
        {
            var placeGameObj = (iPlace as Place)?.gameObject;
            if (!placeGameObj)
                return default(T);

            return placeGameObj.GetOrAddComponent<T>()?.Initialize(iPlace) as T;
        }

        public void Start()
        {
            _dropCurrency?.StartDrop();
            _speechBubble?.Activate();
        }

        public void End()
        {
            _dropCurrency?.StopDrop();
            _speechBubble?.Deactivate();
        }
    }
}

