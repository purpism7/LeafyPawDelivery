using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI;

namespace GameSystem
{
    public class UICreator<T, V> : BaseCreator<T> where T : UI.Base<V> where V : BaseData
    {
        private V _data = null;
        private RectTransform _rootRecTm = null;

        public UICreator<T, V> SetData(V vData)
        {
            _data = vData;

            return this;
        }

        public UICreator<T, V> SetRootRectTm(RectTransform rootRectTm)
        {
            _rootRecTm = rootRectTm;

            return this;
        }

        public override T Create()
        {
            var popup = UIManager.Instance.Instantiate<T>(_rootRecTm);
            if (popup == null)
            {
                return default(T);
            }

            popup.Init(_data);

            return popup;
        }
    }
}
