using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class ObjectManager : Manager.BaseElement<ObjectManager.Data>, IStarter
    {
        public class Data : Game.Manager.BaseData
        {
            public int PlaceId = 0;
        }

        public static UnityEvent<Game.Event.ObjectData> Listener { get; private set; } = new();

        private Data _data = null;
        private Info.ObjectHolder _objectHolder = new();
            
        public List<Info.Object> ObjectInfoList
        {
            get
            {
                return _objectHolder?.GetObjectInfoList(_data.PlaceId);
            }
        }

        protected override void Initialize()
        {
            Listener?.RemoveAllListeners();

            _objectHolder?.LoadInfo();
        }

        public override IEnumerator CoInitialize(Data data)
        {
            _data = data;

            yield break;
        }

        public override void Add(int id)
        {
            if (_objectHolder == null)
                return;

            if (_objectHolder.AddObjectInfo(id))
            {
                Listener?.Invoke(
                    new Event.ObjectData()
                    {
                        Id = id,
                    });
            }
        }

        public override void Remove(int id, int uId)
        {
            _objectHolder?.Remove(id, uId);
        }

        public override bool CheckExist(int objectId)
        {
            if (_data == null)
                return false;

            if (ObjectInfoList == null)
                return false;

            return GetObjectInfoById(objectId) != null;
        }

        public Info.EditObject GetAddEditObject(int id)
        {
            return _objectHolder?.GetAddEditObject(id);
        }

        public void ArrangeObject(int id, int objectUId, Vector3 pos, int placeId)
        {
            _objectHolder?.ArrangeObject(id, objectUId, pos, placeId);
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

        void IStarter.Check()
        {
            if (_data == null)
                return;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            var openConditionDatas = ObjectOpenConditionContainer.Instance?.Datas;
            if (openConditionDatas == null)
                return;

            var objectContainer = ObjectContainer.Instance;

            foreach (var data in openConditionDatas)
            {
                if (data == null)
                    continue;

                var objectData = objectContainer?.GetData(data.Id);
                if (objectData != null &&
                    objectData.PlaceId != _data.PlaceId)
                    continue;

                if (CheckExist(data.Id))
                    continue;

                if (data.EType_ == OpenConditionData.EType.Starter)
                {
                    Sequencer.EnqueueTask(
                        () =>
                        {
                            var popup = new GameSystem.PopupCreator<UI.Obtain, UI.Obtain.Data>()
                                .SetData(new UI.Obtain.Data()
                                {
                                    EElement = Type.EElement.Object,
                                    Id = data.Id,
                                    ClickAction = () =>
                                    {

                                    },
                                })
                                .SetCoInit(true)
                                .SetReInitialize(true)
                                .Create();

                            return popup;
                        });

                    Add(data.Id);
                }
            }
        }
    }
}

