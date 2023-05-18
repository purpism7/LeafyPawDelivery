using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI
{
    public class Arrangement : Base<Arrangement.Data>
    {
        public class Data : BaseData
        {

        }

        public override IEnumerator CoInit(Data data)
        {
            yield return StartCoroutine(base.CoInit(data));

            Debug.Log("Arrangment");
        }

        private void SetAnimalList()
        {
            // GameData.AnimalContainer
        }
        
        public override void Hide()
        {
            base.Hide();
        }
    }
}