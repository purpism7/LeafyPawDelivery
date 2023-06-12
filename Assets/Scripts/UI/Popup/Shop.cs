using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Shop : BasePopup<Shop.Data>
    {
        public class Data : BaseData
        {

        }

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
    }
}
