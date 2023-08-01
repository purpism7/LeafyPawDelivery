using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Element : Base
    {
        public interface IListener
        {
            void Remove();
            void Arrange();
        }

        #region Inspector
        public RectTransform CanvasRectTm = null;
        public RectTransform EditRectTmRoot = null;
        #endregion

        private IListener _iListener = null;

        public void Init(IListener iListener)
        {
            _iListener = iListener;
        }

        public void OnClickRemove()
        {
            _iListener?.Remove();
        }

        public void OnClickArrange()
        {
            _iListener?.Arrange();
        }
    }
}

