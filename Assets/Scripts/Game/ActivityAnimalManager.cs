using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UI;

namespace GameSystem
{
    public class ActivityAnimalManager : GameSystem.Processing
    {
        public delegate void SelectActivityAnimalDelegate(int id);

        public RectTransform RootRectTm;

        private Dictionary<int, ActivityAnimal> _activityAnimalDic = new();
        private event SelectActivityAnimalDelegate _selectActivityAnimalDel = null;

        public override IEnumerator CoProcess(IPreprocessingProvider iProvider)
        {
            //var placeMgr = iProvider?.Get<Game.PlaceManager>();
            //if(placeMgr != null)
            //{
            //    //AddSelectActivityAnimalDelegate(placeMgr.EnableActivityArea);
            //}

            yield break;
        }

        //public void AddSelectActivityAnimalDelegate(SelectActivityAnimalDelegate del)
        //{
        //    if(del == null)
        //    {
        //        return;
        //    }

        //    RemoveSelectActivityAnimalDelegate(del);

        //    _selectActivityAnimalDel += new SelectActivityAnimalDelegate(del);
        //}

        public void RemoveSelectActivityAnimalDelegate(SelectActivityAnimalDelegate del)
        {
            _selectActivityAnimalDel -= del;
        }


        public void CreateActivityAnimal(int animalId)
        {
            //var animalData = GameManager.Instance?.DataContainer?.GetAnimal(animalId);
            //if(animalData == null)
            //{
            //    return;
            //}

            //var activityAnimal = new ActivityAnimalCreator()
            //    .SetAnimalId(animalId)
            //    .SetAnimalName(animalData.name)
            //    .SetRoot(RootRectTm)
            //    .SetSelectActivityAnimal(_selectActivityAnimalDel)
            //    .Create();

            //if(activityAnimal == null)
            //{
            //    return;
            //}

            //_activityAnimalDic.TryAdd(animalId, activityAnimal);
        }

        public void RemoveActivityAnimal(int animalId)
        {
            if(_activityAnimalDic == null)
            {
                return;
            }

            if(_activityAnimalDic.TryGetValue(animalId, out ActivityAnimal activityAnimal))
            {
                Destroy(activityAnimal?.gameObject);
                _activityAnimalDic.Remove(animalId);
            }
        }
    }
}

