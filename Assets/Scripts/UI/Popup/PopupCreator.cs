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
        private bool _showBackground = true;
        private float _animlActivateInterval = 0;
        private bool _forTutorial = false;
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

        public PopupCreator<T, V> SetShowBackground(bool showBackground)
        {
            _showBackground = showBackground;

            return this;
        }

        public PopupCreator<T, V> SetAnimActivateInterval(float interval)
        {
            _animlActivateInterval = interval;

            return this;
        }

        public PopupCreator<T, V> SetForTutorial(bool forTutorial)
        {
            _forTutorial = forTutorial;

            return this;
        }

        public override T Create()
        {
            var initData = new UI.Popup.InitData()
            {
                coInitialzie = _coInitialize,
                reInitialize = _reInitialize,
                animActivate = _animActivate,
                showBackground = _showBackground,
                animActivateInterval = _animlActivateInterval,
                forTutorial = _forTutorial,
            };

            var popup = Game.UIManager.Instance?.Popup?.Instantiate<T, V>(_data, initData);
            if (popup == null)
                return default(T);

            return popup;
        }
    }
}
                                    
