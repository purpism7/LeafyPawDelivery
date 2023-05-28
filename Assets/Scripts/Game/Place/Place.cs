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
        }

        [SerializeField]
        private Transform objectRootTm;

        public Transform ObjectRootTm { get { return objectRootTm; } }

        private List<Game.Object> _objectList = new();
        //public Transform ActivityAreaRootTm;

        //private Dictionary<int, ActivityArea> _activityAreaDic = new();
        //private System.Action<int> _placeActivityAnimalAction = null;
        //private int _selectedAnimalId = 0;

        public override void Init(Data data)
        {
            base.Init(data);

            //_placeActivityAnimalAction = data?.PlaceActivityAnimalAction;

            //InitActivityAreaDic();
            SetObjectList();
        }

        public override void ChainUpdate()
        {
            return;
        }

        public void AddObject(Game.Object obj)
        {
            if (_objectList == null)
                return;

            var findObject = _objectList.Find(obj => obj.UId == obj.ObjectUId);
            if (findObject != null)
                return;

            _objectList.Add(obj);
        }

        public void RemoveObject(int objectUId)
        {
            if(_objectList == null)
                return;

            var findObject = _objectList.Find(obj => obj.UId == objectUId);
            if(findObject == null)
                return;

            if (_objectList.Remove(findObject))
            {
                findObject.Deactivate();
            }
        }

        private void SetObjectList()
        {
            _objectList.Clear();

            var objectInfoList = GameSystem.GameManager.Instance?.ObjectMgr?.ObjectInfoList;
            if (objectInfoList == null)
            {
                return;
            }

            foreach (var objectInfo in objectInfoList)
            {
                if (objectInfo == null)
                    continue;

                if (objectInfo.PlaceId != Id)
                    continue;

                var objData = new Game.Object.Data()
                {
                    ObjectUId = objectInfo.UId,
                    Pos = objectInfo.Pos,
                };

                var obj = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                    .SetData(objData)
                    .SetId(objectInfo.Id)
                    .SetRootTm(objectRootTm)
                    .Create();

                _objectList.Add(obj);
            }
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

