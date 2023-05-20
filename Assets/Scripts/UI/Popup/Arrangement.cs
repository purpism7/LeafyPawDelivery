using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using Data;
using GameData;
using GameSystem;
using UI.Component;
using static UI.Arrangement;

namespace UI
{
    public class Arrangement : Base<Arrangement.Data>
    {
        public class Data : BaseData
        {

        }

        public enum ETabType
        {
            Animal,
            Object,
        }

        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;

        private ETabType _currETabType = ETabType.Animal;

        public override IEnumerator CoInit(Data data)
        {
            yield return StartCoroutine(base.CoInit(data));
            
            SetAnimalList();
            SetObjectList();

            ActiveContents();
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
                   .SetRootRectTm(objectScrollRect?.content)
                   .Create();
            }
        }

        private void ActiveContents()
        {
            UIUtils.SetActive(animalScrollRect?.gameObject, _currETabType == ETabType.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, _currETabType == ETabType.Object);
        }

        public override void DeActivate()
        {
            base.DeActivate();
        }

        public void OnChanged(string tabType)
        {
            if(System.Enum.TryParse(tabType, out ETabType eTabType))
            {
                if(_currETabType == eTabType)
                {
                    return;
                }

                _currETabType = eTabType;

                ActiveContents();
            }
        }
    }
}