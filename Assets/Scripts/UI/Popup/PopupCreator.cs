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

            ShowAnimPopup(popup: popup);

            return popup;
        }

        private void ShowAnimPopup(UI.Base popup)
        {
            if(popup == null)
            {
                return;
            }

            UIManager.Instance.Fade.Out(null,
                () =>
                {

                });

            Sequence sequence = DOTween.Sequence()
                .Append(popup.transform.DOScale(0, 0))
                .Append(popup.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        }
    }
}

