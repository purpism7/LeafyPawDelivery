using System.Collections;
using System.Collections.Generic;
using Data;
using GameData;
using GameSystem;
using UI.Component;
using UnityEngine;
using UnityEngine.Rendering;

namespace UI
{
    public class Arrangement : Base<Arrangement.Data>
    {
        public class Data : BaseData
        {

        }

        [SerializeField] private RectTransform arrangementAnimalCellRootRectTm = null;
        [SerializeField] private RectTransform arrangementObjectCellRootRectTm = null;

        public override IEnumerator CoInit(Data data)
        {
            yield return StartCoroutine(base.CoInit(data));
            
            SetAnimalList();
            SetObjectList();
        }

        private void SetAnimalList()
        {
            //var datas = AnimalContainer.GetDatas;
            //if (datas == null)
            //    return;
            
            //foreach (var data in datas)
            //{
            //    var cell = new ComponentCreator<ArrangementAnimalCell, ArrangementAnimalCell.Data>()
            //        .SetData(new ArrangementAnimalCell.Data()
            //        {
            //            animalData = data,
            //        })
            //        .SetRootRectTm(arrangementAnimalCellRootRectTm)
            //        .Create();
            //}
        }

        private void SetObjectList()
        {
            var infos = GameManager.Instance?.ObjectMgr?.ObjectInfoList;
            if (infos == null)
                return;

            foreach (var info in infos)
            {
                if (info == null)
                    continue;

                var cell = new ComponentCreator<ArrangementObjectCell, ArrangementObjectCell.Data>()
                   .SetData(new ArrangementObjectCell.Data()
                   {
                       //animalData = data,
                   })
                   .SetRootRectTm(arrangementObjectCellRootRectTm)
                   .Create();
            }

        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}