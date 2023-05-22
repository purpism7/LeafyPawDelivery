using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Book : Base<Book.Data>
    {
        public class Data : BaseData
        {

        }
        
        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;

        private Type.ETab _currETabType = Type.ETab.Animal;

        public override void Init(Data data)
        {
            base.Init(data);
            
            
        }
        
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

            // foreach (var info in infos)
            // {
            //     if (info == null)
            //         continue;
            //
            //     var component = new ComponentCreator<ArrangementObjectCell, ArrangementObjectCell.Data>()
            //         .SetData(new ArrangementObjectCell.Data()
            //         {
            //             IListener = this,
            //             ObjectData = ObjectContainer.Instance.GetData(info.Id),
            //             ObjectUId = info.UId,
            //         })
            //         .SetRootRectTm(objectScrollRect?.content)
            //         .Create();
            // }
        }

        private void ActiveContents()
        {
            UIUtils.SetActive(animalScrollRect?.gameObject, _currETabType == Type.ETab.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, _currETabType == Type.ETab.Object);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
        
        public void OnChanged(string tabType)
        {
            if(System.Enum.TryParse(tabType, out Type.ETab eTabType))
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