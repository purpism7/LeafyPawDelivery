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

        private bool _coInit = false;
        // private RectTransform _rootRecTm = null;
        
        public PopupCreator<T, V> SetData(V vData)
        {
            _data = vData;

            return this;
        }

        public PopupCreator<T, V> SetCoInit(bool CoInit)
        {
            _coInit = CoInit;

            return this;
        }

        // 반환값이 의미가 없다 ?!
        public override T Create()
        {
            var popup = UIManager.Instance?.Popup?.Instantiate<T, V>(_data, _coInit);
            if (popup == null)
            {
                return default(T);
            }
                                                                    
            return default(T);
        }
    }
}
                                    
