using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;
using UnityEditor.AddressableAssets.Build.Layout;

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

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            //_placeActivityAnimalAction = data?.PlaceActivityAnimalAction;

            //InitActivityAreaDic();
            SetObjectList();
        }

        public override void ChainUpdate()
        {
            return;
        }

        public void AddObject(Game.Object addObj)
        {
            if (_objectList == null)
                return;

            var findObject = _objectList.Find(obj => obj.UId == addObj.ObjectUId);
            if (findObject != null)
                return;

            _objectList.Add(addObj);
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

            var objectInfoList = MainGameManager.Instance?.ObjectMgr?.ObjectInfoList;
            if (objectInfoList == null)
                return;

            for (int i = 0; i < objectInfoList.Count; ++i)
            {
                var objectInfo = objectInfoList[i];
                if(objectInfo == null)
                    continue;
                
                var data = ObjectContainer.Instance.GetData(objectInfo.Id);
                if(data == null)
                    continue;
                
                if (data.PlaceId != Id)
                    continue;
                
                if(!objectInfo.Arrangement)
                    continue;
                
                var objectData = new Game.Object.Data()
                {
                    ObjectId = objectInfo.Id,
                    ObjectUId = objectInfo.UId,
                    Pos = objectInfo.Pos,
                };

                Game.Object resObj = null;
                foreach(var obj in _objectList)
                {
                    if (obj == null)
                        continue;

                    if (obj.IsActivate)
                        continue;

                    if (objectInfo.Id != obj.Id)
                        continue;

                    resObj = obj;
                    resObj?.Initialize(objectData);

                    break;
                }

                if(resObj == null)
                {
                    resObj = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                        .SetData(objectData)
                        .SetId(objectInfo.Id)
                        .SetRootTm(objectRootTm)
                        .Create();

                    _objectList.Add(resObj);
                }

                resObj?.Activate();
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

