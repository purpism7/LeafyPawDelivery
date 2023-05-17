using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Shop : Base<Shop.Data>
    {
        public class Data : BaseData
        {

        }

        public override IEnumerator CoInit(Data data)
        {
            yield return StartCoroutine(base.CoInit(data));
            
            // ShowAnim();
            
            Debug.Log("Shop");
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
