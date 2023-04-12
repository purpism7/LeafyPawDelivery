using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Edit : Base<Edit.Data>
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
        private RectTransform objectRootRectTm;

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

                var data = new Component.Edit.Data()
                {
                    ObjectId = objectInfo.Id,
                    ObjectUId = objectInfo.UId,
                };

                new GameSystem.UICreator<UI.Component.Edit, UI.Component.Edit.Data>()
                    .SetData(data)
                    .SetRootRectTm(objectRootRectTm)
                    .Create();
            }
        }

        public void OnClickClose()
        {
            _data?.IListener?.Close();
        }
    }
}

