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
            void Remove();
            void Arrange();
        }

        #region Inspector
        public RectTransform CanvasRectTm = null;
        #endregion

        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }

        public void OnClickRemove()
        {
            _data?.IListener?.Remove();
        }

        public void OnClickArrange()
        {
            _data?.IListener?.Arrange();
        }
    }
}

