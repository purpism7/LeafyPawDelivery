using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlaceEvent
{
    public class HiddenObject : Base
    {
        public override Base Initialize(IPlace iPlace, IListener iListener, int placeId)
        {
            base.Initialize(iPlace, iListener, placeId);

            StoryManager.Event?.AddListener(OnChangedStory);

            ArrangeHiddenObject(placeId);

            return this;
        }

        private void ArrangeHiddenObject(int placeId)
        {
            if (CheckExistHiddenObject)
                return;

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

                if (data.ReqStoryId > lastStoryId)
                    continue;

                CreateHiddenObject(data);

                break;
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

                if (!iObject.HiddenObjectRootTm)
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

            if(hiddenObject)
            {
                var localPos = hiddenObject.transform.localPosition;
                hiddenObject.transform.localPosition = new Vector3(localPos.x, localPos.y, -10f);
            }
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

            int placeId = GameUtils.ActivityPlaceId;

            ArrangeHiddenObject(placeId);
        }
        #endregion
    }
}
