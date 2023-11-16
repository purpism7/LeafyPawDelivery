using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

        public UnityEvent<int> Listener { get; private set; } = new();

        protected override void Initialize()
        {
            Debug.Log("ObjectManager Initialize");
            
            Listener?.RemoveAllListeners();

            _objectHolder?.LoadInfo();
        }

        public override IEnumerator CoInitialize(Data data)
        {
            _data = data;

            yield break;
        }

        public void AddObject(int id)
        {
            if (_objectHolder == null)
                return;

            if (_objectHolder.AddObjectInfo(id))
            {
                Listener?.Invoke(id);
            }
        }

        public Info.EditObject GetAddEditObject(int id)
        {
            return _objectHolder?.GetAddEditObject(id);
        }

        public void RemoveObject(int objectId, int objectUId)
        {
            _objectHolder?.RemoveObject(objectId, objectUId);
        }

        public void ArrangeObject(int id, int objectUId, Vector3 pos, int placeId)
        {
            _objectHolder?.ArrangeObject(id, objectUId, pos, placeId);
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

        public int GetRemainCount(int id)
        {
            if (_objectHolder == null)
                return 0;

            return _objectHolder.GetRemainCount(id);
        }
    }
}

