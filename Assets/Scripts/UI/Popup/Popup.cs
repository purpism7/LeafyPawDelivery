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

        public UI.Base Instantiate<T>(out bool init)
        {
            UI.Base basePopup = null;
            init = false;

            if(_opendPopupList != null)
            {
                if(CheckGetOpendPopup<T>(out basePopup))
                {
                    return basePopup;
                }
            }

            var gameObj = ResourceManager.Instance.InstantiateUIGameObj<T>(RootRectTm);
            if(gameObj)
            {
                basePopup = gameObj.GetComponent<UI.Base>();
                _opendPopupList.Add(basePopup);
            }

            init = true;

            return basePopup;
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
                {
                    continue;
                }

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

