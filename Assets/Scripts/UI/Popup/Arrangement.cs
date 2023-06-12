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
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

namespace UI
{
    public class Arrangement : BasePopup<Arrangement.Data>, ArrangementObjectCell.IListener, ArrangementCell.IListener
    {
        public class Data : BaseData
        {

        }

        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;

        private Type.ETab _currETabType = Type.ETab.Animal;

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));
            
            SetAnimalList();
            SetObjectList();

            ActiveContents();
        }

        private void SetAnimalList()
        {
            var datas = AnimalContainer.Instance.Datas;
            if (datas == null)
                return;

            var animalMgr = MainGameManager.Instance?.AnimalMgr;
            if (animalMgr == null)
                return;
   
            foreach (var data in datas)
            {
                if (data == null)
                    continue;

                var animalInfo = animalMgr.GetAnimalInfo(data.Id);

                var component = new ComponentCreator<ArrangementCell, ArrangementCell.Data>()
                    .SetData(new ArrangementCell.Data()
                    {
                        IListener = this,
                        Id = data.Id,
                        Name = data.Name,
                        IconSprite = ResourceManager.Instance?.AtalsLoader?.GetAnimalIconSprite(data.ArrangementIconImg),
                        EState = animalInfo != null ? (animalInfo.Own ? ArrangementCell.EState.Own : ArrangementCell.EState.None) : ArrangementCell.EState.Lock,
                    })
                    .SetRootRectTm(animalScrollRect.content)
                    .Create();
            }
        }

        private void SetObjectList()
        {
            var datas = ObjectContainer.Instance.Datas;
            if (datas == null)
                return;

            var objectMgr = MainGameManager.Instance?.ObjectMgr;
            if (objectMgr == null)
                return;
            
            foreach (var data in datas)
            {
                if (data == null)
                    continue;

                var objectInfo = objectMgr.GetObjectInfoById(data.Id);

                var component = new ComponentCreator<ArrangementCell, ArrangementCell.Data>()
                  .SetData(new ArrangementCell.Data()
                  {
                      IListener = this,
                      Id = data.Id,
                      Name = data.Name,
                      IconSprite = ResourceManager.Instance?.AtalsLoader?.GetObjectIconSprite(data.ArrangementIconImg),
                      EState = objectInfo != null ? (objectInfo.Own ? ArrangementCell.EState.Own : ArrangementCell.EState.None) : ArrangementCell.EState.Lock,
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
            
            Game.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom(Type.ETab.Object);
        }
        #endregion
        
        #region ArrangementAnimalCell.IListener

        void ArrangementCell.IListener.Edit(int id)
        {
            Deactivate();
            
            Game.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom(Type.ETab.Animal);
        }
        #endregion
    }
}