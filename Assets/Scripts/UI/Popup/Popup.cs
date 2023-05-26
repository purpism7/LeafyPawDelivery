using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;

namespace UI
{
    public class Popup : Base
    {
        public RectTransform RootRectTm;

        private List<UI.Base> _opendPopupList = new();
        
        public T Instantiate<T, V>(V vData, bool coInit) 
            where T :UI.Base<V> where V : BaseData
        {
            T resPopup = null;

            if(_opendPopupList != null)
            {
                if(CheckGetOpendPopup<T>(out UI.Base basePopup))
                {
                    resPopup = basePopup.GetComponent<T>();
                    resPopup?.Activate();

                    return resPopup;
                }
            }
            
            StartCoroutine(CoInstantiate<T, V>(
                (popup) =>
                {
                    resPopup = popup;

                }, vData, coInit));
            
            return resPopup;
        }
        
        private IEnumerator CoInstantiate<T, V>(System.Action<T> returnAction, V vData, bool coInit)  
            where T :UI.Base<V> where V : BaseData
        {
            var gameObj = ResourceManager.Instance.InstantiateUIGameObj<T>(RootRectTm);
            if(!gameObj)
                yield break;    
            
            var basePopup = gameObj.GetComponent<UI.Base>();
            _opendPopupList.Add(basePopup);

            var popup = basePopup.GetComponent<T>();
            if (popup == null)
                yield break;
                
            popup.Deactivate();
                
            returnAction?.Invoke(popup);

            if (coInit)
            {
                yield return StartCoroutine(popup.CoInit(vData));
            }
            else
            {
                popup.Init(vData);
            }
               
            popup?.Activate();
        }

        private bool CheckGetOpendPopup<T>(out UI.Base basePopup)
        {
            basePopup = null;

            if(_opendPopupList == null)
            {
                _opendPopupList = new();

                return false;
            }

            foreach(var opendPopup in _opendPopupList)
            {
                if(opendPopup == null)
                    continue;

                if(opendPopup.GetType().Equals(typeof(T)))
                {
                    basePopup = opendPopup;

                    return true;
                }
            }

            return false;
        }
    }
}

