using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace GameSystem
{
    public class PopupCreator<T, V> : BaseCreator<T> where T : UI.Base<V> where V : UI.Data
    {
        private V _data = null;
        private RectTransform _rootRecTm = null;

        public PopupCreator<T, V> SetData(V vData)
        {
            _data = vData;

            return this;
        }

        //public PopupCreator<T, V> SetRootRectTm(RectTransform rootRectTm)
        //{
        //    _rootRecTm = rootRectTm;

        //    return this;
        //}

        public override T Create()
        {
            var popup = UIManager.Instance.InstantiatePopup<T>();
            if (popup == null)
            {
                return default(T);
            }

            popup.Init(_data);            

            return popup;
        }
    }
}

