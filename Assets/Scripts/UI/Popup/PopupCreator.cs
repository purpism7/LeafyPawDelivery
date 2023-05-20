using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI;
using UnityEngine;

namespace GameSystem
{
    public class PopupCreator<T, V> : BaseCreator<T> where T : UI.Base<V> where V : BaseData
    {
        private V _data = null;
        // private RectTransform _rootRecTm = null;

        public PopupCreator<T, V> SetData(V vData)
        {
            _data = vData;

            return this;
        }

        public override T Create()
        {
            bool init = false;
            var basePopup = UIManager.Instance?.Popup?.Instantiate<T>(out init);
            if (basePopup == null)
            {
                return default(T);
            }

            var popup = basePopup.GetComponent<T>();
            if(popup == null)
            {
                return default(T);
            }

            if(init)
            {
                popup.Init(_data);

                StaticCoroutine.Start(popup.CoInit(_data));
            }

            popup.Activate();

            return popup;
        }
    }
}

