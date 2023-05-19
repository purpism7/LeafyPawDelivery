using System.Collections;
using System.Collections.Generic;
using Data;
using GameData;
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
            
            SetAnimalList();
        }

        private void SetAnimalList()
        {
            var datas = AnimalContainer.GetDatas;
            if (datas == null)
                return;
            
            foreach (var data in datas)
            {
                
            }
        }
        
        public override void Hide()
        {
            base.Hide();
        }
    }
}