using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

using GameSystem;
using UnityEngine.UI;

namespace UI
{
    public class Popup : Base
    {
        public RectTransform popupRootRectTm;
        [SerializeField] private Image backgroundImg = null;
        
        private List<UI.Base> _opendPopupList = new();
        private UI.Base _currPopup = null;
        
        public T Instantiate<T, V>(V vData, bool coInit) 
            where T :UI.Base<V> where V : BaseData
        {
            T resPopup = null;

            if(_opendPopupList != null)
            {
                if(CheckGetOpendPopup<T>(out UI.Base uiBase))
                {
                    var basePopup = uiBase.GetComponent<UI.BasePopup<V>>();
                    if (basePopup != null)
                    {
                        resPopup = basePopup.GetComponent<T>();
                        // resPopup?.Activate();
                        
                        FadeOutBackground();
                        basePopup.AnimActivate();
                    }
                    
                    _currPopup = uiBase;
                    
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
            var gameObj = ResourceManager.Instance.InstantiateUIGameObj<T>(popupRootRectTm);
            if(!gameObj)
                yield break;    
            
            var uiBase = gameObj.GetComponent<UI.Base>();
            _opendPopupList.Add(uiBase);

            var basePopup = uiBase.GetComponent<UI.BasePopup<V>>();
            if (basePopup == null)
                yield break;
            
            var popup = basePopup.GetComponent<T>();
            if (popup == null)
                yield break;
                
            basePopup.Deactivate();
                
            returnAction?.Invoke(popup);

            if (coInit)
            {
                yield return StartCoroutine(popup.CoInitialize(vData));
            }
            else
            {
                popup.Initialize(vData);
            }

            FadeOutBackground();
            basePopup?.AnimActivate();
            
            _currPopup = uiBase;
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

        #region Background
        private void FadeOutBackground()
        {
            if (backgroundImg == null)
                return;
            
            UIUtils.SetActive(backgroundImg.gameObject, true);
            
            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(backgroundImg.DOFade(0, 0))
                .Append(backgroundImg.DOFade(0.7f, 0.5f))
                .OnComplete(() =>
                {
                    
                });
            
            sequence.Restart();
        }

        public void DeactivateBackground()
        {
            if (!backgroundImg)
                return;
            
            UIUtils.SetActive(backgroundImg?.gameObject, false);
        }
        
        public void OnClickBackground()
        {
            _currPopup?.Deactivate();
        }
        #endregion
    }
}

