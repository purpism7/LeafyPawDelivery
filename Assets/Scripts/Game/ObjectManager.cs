using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game 
{
    public class ObjectManager : Manager.Base<ObjectManager.Data>
    {
        public class Data : Game.Manager.BaseData
        {
            public int PlaceId = 0;
        }

        private Data _data = null;
        private Info.ObjectHolder _objectHolder = new();

        public List<Info.Object> ObjectInfoList
        {
            get
            {
                return _objectHolder?.GetObjectInfoList(_data.PlaceId);
            }
        }

        public override IEnumerator CoInit(Data data)
        {
            _data = data;
            //_objectInfoList.Clear();
            _objectHolder?.LoadInfo();
            //_objectInfoList.AddRange(objectHolder.ObectInfoList);

            yield break;
        }

        public void RemoveObject(int objectId, int objectUId)
        {
            _objectHolder?.RemoveObject(objectId, objectUId);
        }

        public void ArrangeObject(int objectUId, Vector3 pos, int placeId)
        {
            _objectHolder?.ArrangeObject(objectUId, pos, placeId);
        }

        public void AddObject(int objectId)
        {
            var objectInfo = new Info.Object()
            {
                Id = objectId,
            };

            _objectHolder?.AddObject(objectInfo);
        }

        public bool CheckExist(int objectId)
        {
            if (_data == null)
                return false;
            
            if (ObjectInfoList == null)
                return false;
            
            return GetObjectInfoById(objectId) != null;
        }

        public Info.Object GetObjectInfoById(int objectId)
        {
            if (_data == null)
                return null;
            
            return _objectHolder.GetObjectInfoById(objectId, _data.PlaceId);
        }
    }
}

