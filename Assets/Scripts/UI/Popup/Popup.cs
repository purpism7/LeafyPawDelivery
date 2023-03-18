using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Popup : Base
    {
        public RectTransform RootRectTm;

        private List<UI.Base> _opendPopupList = new();
    }
}

