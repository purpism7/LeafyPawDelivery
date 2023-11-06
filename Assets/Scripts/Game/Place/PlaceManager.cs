using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using GameSystem;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class PlaceManager : Manager.Base<PlaceManager.Data>
    {
        public class Data : Manager.BaseData
        {
            public int PlaceId = 0;
        }

        public static UnityEvent<int> Listener { get; private set; } = null;

        public Transform RootTm;

        private List<Place> _placeList = new List<Place>();

        public Place ActivityPlace { get; private set; } = null;
        public int ActivityPlaceId 
        {
            get
            {
                return ActivityPlace != null ? ActivityPlace.Id : 0;
            }
        }

        protected override void Initialize()
        {
            Listener = new UnityEvent<int>();
            Listener?.RemoveAllListeners();
        }

        public override IEnumerator CoInitialize(Data data)
        {
            SetActivityPlace(data);

            yield break;
        }

        private void SetActivityPlace(Data data)
        {
            ActivityPlace?.Deactivate();

            var place = _placeList?.Find(place => place.Id == data.PlaceId);
            if (place != null)
            {
                ActivityPlace = place;
            }
            else
            {
                ActivityPlace = new GameSystem.PlaceCreator()
                  .SetPlaceId(data.PlaceId)
                  .SetRoot(RootTm)
                  .Create();

                _placeList?.Add(ActivityPlace);
            }

            Listener?.Invoke(ActivityPlaceId);
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            ActivityPlace?.ChainUpdate();
        }

        //public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        //{
        //    //var activityAnimalMgr = iProvider?.Get<ActivityAnimalManager>();
        //    //if (activityAnimalMgr != null)
        //    //{
        //    //    _removeActivityAnimalAction = activityAnimalMgr.RemoveActivityAnimal;
        //    //}

        //ActivityPlace = new GameSystem.PlaceCreator()
        //        .SetPlaceId(1)
        //        .SetRoot(RootTm)
        //        //.SetPlaceActivityAnimalAction(PlaceActivityAnimal)
        //        .Create();

        //_placeList.Add(ActivityPlace);

        //    yield break;
        //}

        //public void EnableActivityArea(int animalId)
        //{
        //    if(ActivityPlace == null)
        //    {
        //        return;
        //    }

        //    ActivityPlace.EnableActivityArea(animalId);
        //}

        //private void PlaceActivityAnimal(int animalId)
        //{
        //    _removeActivityAnimalAction?.Invoke(animalId);
        //}
    }
}
