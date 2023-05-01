using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using GameSystem;

namespace Game
{
    public class PlaceManager : Manager.Base<PlaceManager.Data>
    {
        public class Data : Manager.BaseData
        {

        }

        public Transform RootTm;

        private List<Place> _placeList = new List<Place>();

        public Place ActivityPlace { get; private set; } = null;

        public override IEnumerator CoInit(Data data)
        {
            Debug.Log("PlaceManager CoInit");

            ActivityPlace = new GameSystem.PlaceCreator()
                .SetPlaceId(1)
                .SetRoot(RootTm)
                .Create();

            _placeList.Add(ActivityPlace);

            yield break;
        }

        public void RemoveObject(int objectUId)
        {
            ActivityPlace?.RemoveObject(objectUId);
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
