using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Component
{
    public class MapIcon : Base<MapIcon.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
        }

        public interface IListener
        {
            void SelectPlace(int id);
        }

        [SerializeField]
        public int placeId = 0;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }

        public void OnClick()
        {
            _data?.IListener?.SelectPlace(placeId);
        }
    }
}

