using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Shop : BasePopup<Shop.Data>
    {
        public class Data : BaseData
        {

        }

        [SerializeField]
        private ScrollRect itemScrollRect = null;

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));
            
            // ShowAnim();
    
            Debug.Log("Shop");
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void SetItemList()
        {

        }
    }
}
