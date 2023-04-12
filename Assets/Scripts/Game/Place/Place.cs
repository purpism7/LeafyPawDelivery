using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;

namespace Game
{
    public class Place : Base<Place.Data>//, ActivityArea.IListener
    {
        public class Data : BaseData
        {
            public int Id = 0;
            //public System.Action<int> PlaceActivityAnimalAction = null;
        }

        //public Transform ActivityAreaRootTm;

        //private Dictionary<int, ActivityArea> _activityAreaDic = new();
        //private System.Action<int> _placeActivityAnimalAction = null;
        //private int _selectedAnimalId = 0;

        public override void Init(Data data)
        {
            base.Init(data);

            //_placeActivityAnimalAction = data?.PlaceActivityAnimalAction;

            //InitActivityAreaDic();
        }

        public override void ChainUpdate()
        {
            return;
        }

        //private void InitActivityAreaDic()
        //{
        //    _activityAreaDic.Clear();

        //    var activityAreas = GetComponentsInChildren<ActivityArea>();
        //    if (activityAreas == null)
        //    {
        //        return;
        //    }

        //    foreach (var activityArea in activityAreas)
        //    {
        //        if (activityArea == null)
        //        {
        //            continue;
        //        }

        //        activityArea.Init(new ActivityArea.Data_()
        //        {
        //            IListener = this,
        //        });

        //        _activityAreaDic.TryAdd(activityArea.Id, activityArea);
        //    }
        //}

        //public void EnableActivityArea(int animalId)
        //{
        //    _selectedAnimalId = animalId;

        //    EnalbeAllActivityArea(true);
        //}

        //private void EnalbeAllActivityArea(bool enable)
        //{
        //    if (_activityAreaDic == null)
        //    {
        //        return;
        //    }

        //    foreach (var activityArea in _activityAreaDic.Values)
        //    {
        //        if (activityArea == null)
        //        {
        //            continue;
        //        }

        //        if(enable &&
        //           activityArea.PlayingAnimal)
        //        {
        //            continue;
        //        }

        //        activityArea.Enable(enable);
        //    }
        //}

        //#region ActivityArea.IListener
        //void ActivityArea.IListener.PlaceAnimal(ActivityArea activityArea)
        //{
        //    if(activityArea == null)
        //    {
        //        return;
        //    }

        //    if(activityArea.PlaceAnimal(_selectedAnimalId))
        //    {
        //        EnalbeAllActivityArea(false);

        //        _placeActivityAnimalAction?.Invoke(_selectedAnimalId);

        //        _selectedAnimalId = 0;
        //    }
        //}
        //#endregion
    }
}

