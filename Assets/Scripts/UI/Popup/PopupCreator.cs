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

        private bool _coInitialize = false;

        private bool _reInitialize = false;
        private bool _animActivate = true;
        // private RectTransform _rootRecTm = null;

        //public static T a<T, V>() where T : UI.Base<V> where V : BaseData
        //{
        //    new PopupCreate<T, V>();
        //}

        public PopupCreator<T, V> SetData(V vData)
        {
            _data = vData;

            return this;
        }

        public PopupCreator<T, V> SetCoInit(bool CoInit)
        {
            _coInitialize = CoInit;

            return this;
        }
        
        public PopupCreator<T, V> SetReInitialize(bool reInitialize)
        {
            _reInitialize = reInitialize;

            return this;
        }

        public PopupCreator<T, V> SetAnimActivate(bool animActivate)
        {
            _animActivate = animActivate;

            return this;
        }

        public override T Create()
        {
            var initData = new UI.Popup.InitData()
            {
                CoInitialzie = _coInitialize,
                ReInitialize = _reInitialize,
                AnimActivate = _animActivate,
            };

            var popup = Game.UIManager.Instance?.Popup?.Instantiate<T, V>(_data, initData);
            if (popup == null)
                return default(T);

            return popup;
        }
    }
}
                                    
