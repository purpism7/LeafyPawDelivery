using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Arrangement : Base<Arrangement.Data>
    {
        public class Data : UI.Data
        {

        }

        public override IEnumerator CoInit(Data data)
        {
            yield return StartCoroutine(base.CoInit(data));

            Debug.Log("Arrangment");
        }
    }
}