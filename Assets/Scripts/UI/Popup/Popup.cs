using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Popup : Base<Popup.Data>
    {
        public class Data : UI.Data
        {

        }

        public RectTransform RootRectTm;

        private List<UI.Base> _opendPopupList = new();
    }
}

