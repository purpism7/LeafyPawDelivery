using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SpecialThanks : BasePopup<BaseData>
    {
        [SerializeField]
        private ScrollRect scrollRect = null;

        public override void Activate()
        {
            base.Activate();

            scrollRect?.ResetScrollPos();
        }
    }
}

