using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameSystem;

namespace Game
{
    public class PlaceManager : Manager.Base
    {
        public Transform RootTm;

        private List<Place> _placeList = new List<Place>();
        //private System.Action<int> _removeActivityAnimalAction = null;

        public Place ActivityPlace { get; private set; } = null;

        public override IEnumerator CoInit()
        {
            Debug.Log("PlaceManager CoInit");

            ActivityPlace = new GameSystem.PlaceCreator()
                .SetPlaceId(1)
                .SetRoot(RootTm)
                //.SetPlaceActivityAnimalAction(PlaceActivityAnimal)
                .Create();

            _placeList.Add(ActivityPlace);

            yield break;
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
