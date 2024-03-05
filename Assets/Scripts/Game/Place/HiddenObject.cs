using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public class HiddenObject : Base
    {
        private List<int> _idList = null;

        public override Base Initialize(IPlace iPlace, IListener iListener, int placeId)
        {
            base.Initialize(iPlace, iListener, placeId);

            StoryManager.Event?.AddListener(OnChangedStory);
             
            ArrangeHiddenObject(placeId);

            return this;
        }

        public void Activate()
        {
            var objList = _iPlace?.objectList;
            if (objList == null)
                return;

            for (int i = 0; i < objList.Count; ++i)
            {
                IObject iObj = objList[i];

                iObj?.ActivateHiddenObject();
            }
        }

        public void Deactivate()
        {
            var objList = _iPlace?.objectList;
            if (objList == null)
                return;

            for (int i = 0; i < objList.Count; ++i)
            {
                IObject iObj = objList[i];

                iObj?.DeactivateHiddenObject();
            }
        }

        private void ArrangeHiddenObject(int placeId)
        {
            //if (CheckExistHiddenObject)
            //    return;

            var objMgr = MainGameManager.Get<ObjectManager>();
            if (objMgr == null)
                return;

            var user = Info.UserManager.Instance?.User;
            if (user == null)
                return;

            int lastStoryId = user.GetLastStoryId(placeId);

            var dataList = ObjectOpenConditionContainer.Instance?.GetDataList(new[] { OpenConditionData.EType.Hidden });
            foreach (var data in dataList)
            {
                if (data == null)
                    continue;

                if (objMgr.CheckExist(data.Id))
                    continue;

                if (CheckExist(data.Id))
                    continue;

                var objData = ObjectContainer.Instance?.GetData(data.Id);
                if(objData != null)
                {
                    if (objData.PlaceId != placeId)
                        continue;
                }

                if (data.ReqStoryId > lastStoryId)
                    continue;

                CreateHiddenObject(data);
            }
        }

        private void CreateHiddenObject(OpenConditionData data)
        {
            var objList = _iPlace?.objectList;
            if (objList == null)
                return;

            var iObjectList= new List<IObject>();
            iObjectList.Clear();

            IObject iObject = null;
            for (int i = 0; i < objList.Count; ++i)
            {
                iObject = objList[i];
                if (iObject == null)
                    continue;

                if (!iObject.IsActivate)
                    continue;

                if (!iObject.HiddenObjectRootTm)
                    continue;

                if (iObject.CheckExistHiddenObject)
                    continue;

                iObjectList?.Add(iObject);
            }

            if (iObjectList.Count <= 0)
                return;

            int randIndex = UnityEngine.Random.Range(0, iObjectList.Count);
            var randIObject = iObjectList[randIndex];

            var hiddenObject = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                .SetId(data.Id)
                .SetData(new Object.Data()
                {
                    ObjectId = data.Id,
                    isHiddenObj = true,
                    sortingOrder = randIObject.SortingOrder - 1,
                })
                .SetRootTm(randIObject.HiddenObjectRootTm)
                .Create();

            if(hiddenObject != null)
            {
                var localPos = hiddenObject.transform.localPosition;
                hiddenObject.transform.localPosition = new Vector3(localPos.x, localPos.y, -20f);
            }

            if(AddId(data.Id))
            {
                _iListener?.Action(new HiddenObjectData()
                {
                    id = data.Id,
                    eElement = Type.EElement.Object,
                });
            }
        }

        private bool CheckExist(int checkId)
        {
            if (_idList == null)
                return false;

            return _idList.Find(id => id == checkId) > 0;
        }

        private bool AddId(int addId)
        {
            if(_idList == null)
            {
                _idList = new();
                _idList.Clear();
            }

            if(!CheckExist(addId))
            {
                _idList.Add(addId);

                return true;
            }

            return false;
        }

        private bool CheckExistHiddenObject
        {
            get
            {
                var objList = _iPlace?.objectList;
                if (objList == null)
                    return false;

                for (int i = 0; i < objList.Count; ++i)
                {
                    IObject iObj = objList[i];
                    if (iObj == null)
                        continue;

                    if (iObj.CheckExistHiddenObject)
                        return true;
                }

                return false;
            }
        }

        #region StoryManager.Event
        private void OnChangedStory(Event.StoryData storyData)
        {
            if (storyData == null)
                return;

            if (storyData.EState != Event.EState.End)
                return;

            int placeId = GameUtils.ActivityPlaceId;
            if (_placeId != placeId)
                return;

            ArrangeHiddenObject(placeId);
        }
        #endregion
    }
}
