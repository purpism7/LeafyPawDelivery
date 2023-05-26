using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Creature;
using Data;
using GameData;
using GameSystem;
using UI.Component;
using Unity.VisualScripting;
using static UI.Arrangement;

namespace UI
{
    public class Arrangement : Base<Arrangement.Data>, ArrangementObjectCell.IListener, ArrangementAnimalCell.IListener
    {
        public class Data : BaseData
        {

        }

        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;

        private Type.ETab _currETabType = Type.ETab.Animal;

        public override IEnumerator CoInit(Data data)
        {
            yield return StartCoroutine(base.CoInit(data));
            
            SetAnimalList();
            SetObjectList();

            ActiveContents();
            Debug.Log("Arrangement init");
        }

        private void SetAnimalList()
        {
            var infos = GameManager.Instance?.AnimalMgr?.AnimalInfoList;
            if (infos == null)
                return;
            
            foreach (var info in infos)
            {
                var component = new ComponentCreator<ArrangementAnimalCell, ArrangementAnimalCell.Data>()
                    .SetData(new ArrangementAnimalCell.Data()
                    {
                        IListener = this, 
                        AnimalData = AnimalContainer.Instance.GetData(info.Id),
                    })
                    .SetRootRectTm(animalScrollRect.content)
                    .Create();
            }
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

                 var component = new ComponentCreator<ArrangementObjectCell, ArrangementObjectCell.Data>()
                     .SetData(new ArrangementObjectCell.Data()
                     {
                         IListener = this,
                         ObjectData = ObjectContainer.Instance.GetData(info.Id),
                         ObjectUId = info.UId,
                     })
                     .SetRootRectTm(objectScrollRect.content)
                     .Create();
            }
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
        
        #region ArrangementObjectCell.IListener

        void ArrangementObjectCell.IListener.Edit(int objectUId)
        {
            Deactivate();
            
            GameSystem.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom();
        }
        #endregion
        
        #region ArrangementAnimalCell.IListener

        void ArrangementAnimalCell.IListener.Edit(int animalId)
        {
            Deactivate();
            
            GameSystem.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom();
        }
        #endregion
    }
}