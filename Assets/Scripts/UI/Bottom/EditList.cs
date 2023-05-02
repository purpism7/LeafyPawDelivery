using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class EditList : Base<EditList.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
        }

        public interface IListener
        {
            void Close();
        }

        [SerializeField]
        private RectTransform ObjectRootRectTm;

        private List<Component.EditObject> _editObjectList = new();

        public override void Init(Data data)
        {
            base.Init(data);

            SetObjectList();
        }

        private void SetObjectList()
        {
            _editObjectList.Clear();

            var objectInfoList = GameSystem.GameManager.Instance?.ObjectMgr?.ObjectInfoList;
            if(objectInfoList == null)
            {
                return;
            }

            foreach(var objectInfo in objectInfoList)
            {
                if(objectInfo == null)
                {
                    continue;
                }

                if(objectInfo.PlaceId > 0)
                {
                    continue;
                }

                var data = new Component.EditObject.Data()
                {
                    ObjectId = objectInfo.Id,
                    ObjectUId = objectInfo.UId,
                };

                var editObject = new GameSystem.UICreator<UI.Component.EditObject, UI.Component.EditObject.Data>()
                    .SetData(data)
                    .SetRootRectTm(ObjectRootRectTm)
                    .Create();

                _editObjectList.Add(editObject);
            }
        }

        public void RefreshObjectList()
        {
            var objectInfoList = GameSystem.GameManager.Instance?.ObjectMgr?.ObjectInfoList;
            if (objectInfoList == null)
            {
                return;
            }

            for(int i = 0; i < objectInfoList.Count; ++i)
            {
                var objectInfo = objectInfoList[i];
                if (objectInfo == null)
                {
                    continue;
                }

                if (objectInfo.PlaceId > 0)
                {
                    continue;
                }

                var data = new Component.EditObject.Data()
                {
                    ObjectId = objectInfo.Id,
                    ObjectUId = objectInfo.UId,
                };

                if(_editObjectList != null &&
                   _editObjectList.Count > i)
                {
                    _editObjectList[i].Init(data);
                }
            }

            for(int i = objectInfoList.Count; i < _editObjectList.Count; ++i)
            {
                var editObject = _editObjectList[i];
                if(editObject == null)
                {
                    continue;
                }

                UIUtils.SetActive(editObject.gameObject, false);
            }
        }

        public void OnClickClose()
        {
            _data?.IListener?.Close();
        }
    }
}

