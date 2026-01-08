using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

using Game.Event;
using GameSystem;

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
            public GardenManager GardenManger { get; private set; } = null;

            public Data WithGardenManager(GardenManager manager)
            {
                GardenManger = manager;
                return this;
            }
        }

        public static UnityEvent<Game.Event.PlaceData> Event { get; private set; } = null;

        public Transform RootTm;
        [SerializeField]
        private GameData.Place placeData = null;

        private List<Place> _placeList = new List<Place>();
        private GardenManager _gardenManager = null;

        public Place ActivityPlace { get; private set; } = null;
        public IPlace ActivityIPlace { get { return ActivityPlace; } }
        public int ActivityPlaceId 
        {
            get
            {
                return ActivityPlace != null ? ActivityPlace.Id : 0;
            }
        }

        public override MonoBehaviour Initialize()
        {
            Event = new();
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
                  .SetData(
                      new Place.Data
                      {
                          Id = data.placeId,
                          onBGM = Info.Setting.Get.OnBGM,
                      }.WithGardenManager(_gardenManager))
                  .Create();

                _placeList?.Add(ActivityPlace);
            }

            Event?.Invoke(new PlaceData(ActivityPlaceId));
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            ActivityPlace?.ChainUpdate();
        }

        public void SetAlphaActivityAnimal(float a, int exceptAnimalId = 0)
        {
            IPlace iPlace = ActivityPlace;
            if (iPlace == null)
                return;

            var animalList = iPlace.AnimalList;
            if (animalList == null)
                return;
            
            var color = Color.white;
            color.a = a;
            
            foreach (var animal in animalList)
            {
                if(animal == null)
                    continue;

                if (animal.Id == exceptAnimalId)
                    continue;
                
                animal.SetColor(color);
            }
        }

        public void SetAlphaActivityObject(float a)
        {
            IPlace iPlace = ActivityPlace;
            if (iPlace == null)
                return;

            var objectList = iPlace.ObjectList;
            if (objectList == null)
                return;
            
            var color = Color.white;
            color.a = a;
            
            foreach (var obj in objectList)
            {
                if (obj == null)
                    continue;
                
                obj.SetColor(color);
            }
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
        #endregion
    }
}
