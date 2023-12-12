using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using UnityEngine;

using GameSystem;
using UnityEngine.UI;

namespace UI
{
    public class Popup : Base
    {
        public class InitData
        {
            public bool coInitialzie = false;
            public bool reInitialize = false;
            public bool animActivate = true;
            public bool showBackground = true;
        }

        public RectTransform popupRootRectTm;
        [SerializeField] private Image backgroundImg = null;
        
        private List<UI.Base> _opendPopupList = new();
        private Stack<UI.Base> _popupStack = new();

        public T Instantiate<T, V>(V vData, InitData initData) 
            where T :UI.Base<V> where V : BaseData
        {
            T resPopup = null;

            if (initData == null)
                return resPopup;

            if (_opendPopupList != null)
            {
                if(CheckGetOpendPopup<T>(out UI.Base uiBase))
                {
                    var basePopup = uiBase.GetComponent<UI.BasePopup<V>>();
                    if (basePopup != null)
                    {
                        if (!initData.reInitialize)
                        {
                            resPopup = basePopup.GetComponent<T>();
                        
                            ActivatePopup<T, V>(uiBase, initData.animActivate, initData.showBackground);
                            
                            return resPopup;
                        }
                    }
                }
            }
            
            StartCoroutine(CoInstantiate<T, V>(
                (popup) =>
                {
                    resPopup = popup;

                }, vData, initData));
            
            return resPopup;
        }
        
        private IEnumerator CoInstantiate<T, V>(System.Action<T> returnAction, V vData, InitData initData)  
            where T :UI.Base<V> where V : BaseData
        {
            UI.Base uIBase = null;
            if (!CheckGetOpendPopup<T>(out uIBase))
            {
                var gameObj = ResourceManager.Instance.InstantiateUIGameObj<T>(popupRootRectTm);
                if(!gameObj)
                    yield break;    
            
                uIBase = gameObj.GetComponent<UI.Base>();
                _opendPopupList.Add(uIBase);
            }
            
            var basePopup = uIBase.GetComponent<UI.BasePopup<V>>();
            if (basePopup == null)
                yield break;
            
            var popup = basePopup.GetComponent<T>();
            if (popup == null)
                yield break;
                
            UIUtils.SetActive(basePopup.rootRectTm, false);
                
            returnAction?.Invoke(popup);

            if (initData.coInitialzie)
            {
                yield return StartCoroutine(popup.CoInitialize(vData));
            }
            else
            {
                popup.Initialize(vData);
            }

            ActivatePopup<T, V>(uIBase, initData.animActivate, initData.showBackground);
        }
        
        private void ActivatePopup<T, V>(UI.Base uiBase, bool animActivate, bool showBackground) where T :UI.Base<V> where V : BaseData
        {
            if (uiBase == null)
                return;
            
            uiBase.transform.SetAsLastSibling();
            
            var basePopup = uiBase.GetComponent<UI.BasePopup<V>>();
            if (basePopup == null)
                return;

            FadeOutBackground(showBackground);
            if(animActivate)
            {
                basePopup.AnimActivate();
            }
            else
            {
                basePopup.Activate();
            }

            if (_popupStack.Count > 0)
            {
                UIUtils.SetActive(_popupStack?.Peek()?.rootRectTm, false);
            }
            
            _popupStack?.Push(uiBase);
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
        
        private void Check()
        {
            foreach (RectTransform rectTm in popupRootRectTm)
            {
                if(!rectTm)
                    continue;
                
                Debug.Log(rectTm.name);
            }
        }

        public void PopPopup()
        {
            if (_popupStack.Count <= 0)
                return;

            _popupStack?.Pop();

            if (_popupStack.Count > 0)
            {
                UIUtils.SetActive(_popupStack?.Peek()?.rootRectTm, true);
            }
            else
            {
                DeactivateBackground();
            }
        }

        #region Background
        private void FadeOutBackground(bool show)
        {
            if (backgroundImg.isActiveAndEnabled)
                return;
            
            if (backgroundImg == null)
                return;
            
            UIUtils.SetActive(backgroundImg.gameObject, true);

            if(show)
            {
                Sequence sequence = DOTween.Sequence()
                    .SetAutoKill(false)
                    .Append(backgroundImg.DOFade(0, 0))
                    .Append(backgroundImg.DOFade(0.7f, 0.5f));
                //.OnComplete(() =>
                //{

                //});
                sequence.Restart();
            }
            else
            {
                backgroundImg.DOFade(0, 0);
            }
        }

        private void DeactivateBackground()
        {
            if (_popupStack.Count > 0)
                return;
            
            if (!backgroundImg)
                return;
            
            UIUtils.SetActive(backgroundImg?.gameObject, false);
        }
        
        public void OnClickBackground()
        {
            var uiBase = _popupStack?.Peek();
            if (uiBase == null)
                return;

            uiBase.ClickClose();
            uiBase.Deactivate();
        }
        #endregion
    }
}

