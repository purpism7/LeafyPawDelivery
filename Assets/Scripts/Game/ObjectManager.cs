using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class ObjectManager : Manager.BaseElement<ObjectManager.Data>, IEvent
    {
        public class Data : Game.Manager.BaseData
        {
            public int PlaceId = 0;
        }

        public static UnityEvent<Game.Event.ObjectData> Event { get; private set; } = new();

        private Data _data = null;
        private Info.ObjectHolder _objectHolder = new();
            
        public List<Info.Object> ObjectInfoList
        {
            get
            {
                return _objectHolder?.GetObjectInfoList(_data.PlaceId);
            }
        }

        public override MonoBehaviour Initialize()
        {
            Event?.RemoveAllListeners();

            return this;
        }

        public override IEnumerator CoInitialize(Data data)
        {
            _data = data;

            _objectHolder?.LoadInfo();

            yield break;
        }

        public bool CheckIsAll(int placeId)
        {
            var objectDataList = ObjectContainer.Instance.GetDataListByPlaceId(placeId);
            if (objectDataList == null)
                return false;

            var openConditionDataList = ObjectOpenConditionContainer.Instance?.GetDataList(new[] { OpenConditionData.EType.Starter, OpenConditionData.EType.Buy });
            foreach (var objectData in objectDataList)
            {
                if (objectData == null)
                    continue;

                if (openConditionDataList != null &&
                    openConditionDataList.Find(openConditionData => openConditionData.Id == objectData.Id) != null)
                {
                    if (!CheckExist(objectData.Id))
                        return false;
                }
            }

            return true;
        }

        public bool CheckGetStarter
        {
            get
            {
                int placeId = GameUtils.ActivityPlaceId;
                if (placeId != Games.Data.Const.StartPlaceId)
                    return true;

                var objectDataList = ObjectContainer.Instance?.GetDataListByPlaceId(placeId);
                if (objectDataList == null)
                    return false;

                var openConditionDataList = ObjectOpenConditionContainer.Instance?.GetDataList(new[] { OpenConditionData.EType.Starter });
                foreach (var objectData in objectDataList)
                {
                    if (objectData == null)
                        continue;

                    if (openConditionDataList != null &&
                        openConditionDataList.Find(openConditionData => openConditionData.Id == objectData.Id) != null)
                    {
                        if (!CheckExist(objectData.Id))
                            return false;
                    }
                }

                return true;
            }
        }

        public override void Add(int id)
        {
            if (_objectHolder == null)
                return;

            if (_objectHolder.AddObjectInfo(id))
            {
                OpenConditionData.EType eOpenConditionType = OpenConditionData.EType.None;
                var openConditionData = ObjectOpenConditionContainer.Instance?.GetData(id);
                if(openConditionData != null)
                {
                    eOpenConditionType = openConditionData.eType;
                }

                Event?.Invoke(
                    new Event.AddObjectData()
                    {
                        id = id,
                        eOpenConditionType = eOpenConditionType,
                    });

                Info.UserManager.Instance?.AddObject(id);

                Info.Connector.Get?.SetAddObject(id);

                Notification.Get?.Notify(Notification.EType.OpenPlace);
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

        public void ArrangeObject(Game.Object obj, int placeId)
        {
            if (_objectHolder == null)
                return;

            if(_objectHolder.ArrangeObject(obj, placeId))
            {
                Event?.Invoke(new Game.Event.ArrangeObjectData()
                {
                    id = obj.Id,
                });
            }
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

        void IEvent.Starter(System.Action endAction)
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

                if (data.eType == OpenConditionData.EType.Starter)
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
                                        endAction?.Invoke();
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

