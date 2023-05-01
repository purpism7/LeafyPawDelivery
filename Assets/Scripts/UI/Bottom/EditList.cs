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

        public override void Init(Data data)
        {
            base.Init(data);

            SetObjectList();
        }

        private void SetObjectList()
        {
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

                new GameSystem.UICreator<UI.Component.EditObject, UI.Component.EditObject.Data>()
                    .SetData(data)
                    .SetRootRectTm(ObjectRootRectTm)
                    .Create();
            }
        }

        public void OnClickClose()
        {
            _data?.IListener?.Close();
        }
    }
}

