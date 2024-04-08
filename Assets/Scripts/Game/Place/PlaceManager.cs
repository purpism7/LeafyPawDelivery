using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using GameSystem;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public interface IPlaceData
    {
        GameData.Place.Data GetPlaceData(int placeId);

        int LastPlaceId { get; }
        int TotalPlaceCount { get; }
    }

    public class PlaceManager : Manager.Base<PlaceManager.Data>, IUpdater, IPlaceData
    {
        public class Data : Manager.BaseData
        {
            public int placeId = 0;
        }

        public static UnityEvent<int> Event { get; private set; } = null;

        public Transform RootTm;
        [SerializeField]
        private GameData.Place placeData = null;

        private List<Place> _placeList = new List<Place>();

        public Place ActivityPlace { get; private set; } = null;
        public int ActivityPlaceId 
        {
            get
            {
                return ActivityPlace != null ? ActivityPlace.Id : 0;
            }
        }

        public override MonoBehaviour Initialize()
        {
            Event = new UnityEvent<int>();
            Event?.RemoveAllListeners();

            PlaceEventController.Event?.RemoveAllListeners();

            return this;
        }

        public override IEnumerator CoInitialize(Data data)
        {
            SetActivityPlace(data);

            yield break;
        }

        private void SetActivityPlace(Data data)
        {
            ActivityPlace?.Deactivate();

            var place = _placeList?.Find(place => place.Id == data.placeId);
            if (place != null)
            {
                ActivityPlace = place;
            }
            else
            {
                ActivityPlace = new GameSystem.PlaceCreator()
                  .SetRoot(RootTm)
                  .SetData(new Place.Data()
                  {
                      Id = data.placeId,
                      onBGM = Info.Setting.Get.OnBGM,
                  })
                  .Create();

                _placeList?.Add(ActivityPlace);
            }

            Event?.Invoke(ActivityPlaceId);
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            ActivityPlace?.ChainUpdate();
        }

        public GameData.Place.Data ActivityPlaceData
        {
            get
            {
                if (placeData == null)
                    return null;

                return placeData.GetPlaceData(ActivityPlaceId);
            }
        }


        #region IPlaceData
        GameData.Place.Data IPlaceData.GetPlaceData(int placeId)
        {
            if (placeData == null)
                return null;

            return placeData.GetPlaceData(placeId);
        }

        int IPlaceData.LastPlaceId
        {
            get
            {
                if (placeData == null)
                    return 0;

                return placeData.LastPlaceId;
            }
        }

        int IPlaceData.TotalPlaceCount
        {
            get
            {
                if (placeData == null ||
                    placeData.Datas == null)
                    return 0;

                return placeData.Datas.Length;
            }
        }

        //public Info.User.Currency GetStartCurrency(int placeId)
        //{
        //    return placeData?.GetStartCurrency(placeId);
        //}
        #endregion
    }
}
