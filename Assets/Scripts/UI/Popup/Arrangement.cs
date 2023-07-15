using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using Game.Creature;
using Data;
using GameData;
using GameSystem;
using UI.Component;

namespace UI
{
    public class Arrangement : BasePopup<Arrangement.Data>, ArrangementCell.IListener
    {
        public class Data : BaseData
        {

        }

        [SerializeField] private Toggle[] tabToggles = null;
        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;

        private Type.ETab _currETabType = Type.ETab.Animal;

        private List<ArrangementCell> _arrangementCellList = new();

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));
            
            MainGameManager.Instance?.AnimalMgr?.Listener?.AddListener(OnChangedAnimalInfo);
            MainGameManager.Instance?.ObjectMgr?.Listener?.AddListener(OnChangedObjectInfo);
            
            _arrangementCellList.Clear();
            
            SetAnimalList();
            SetObjectList();
        }

        public override void Activate()
        {
            base.Activate();
            
            _currETabType = Type.ETab.Animal;
            ActiveContents();

            var tabToggle = tabToggles?.First();
            if (tabToggle != null)
            {
                tabToggle.SetIsOnWithoutNotify(true);
            }
        }
        
        public override void Deactivate()
        {
            base.Deactivate();
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

                var cell = new ComponentCreator<ArrangementCell, ArrangementCell.Data>()
                    .SetData(new ArrangementCell.Data()
                    {
                        IListener = this,
                        
                        Id = data.Id,
                        EMain = Type.EMain.Animal,
                        Name = data.Name,
                        Lock = animalInfo == null,
                    })
                    .SetRootRectTm(animalScrollRect.content)
                    .Create();
                
                _arrangementCellList.Add(cell);
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

                var cell = new ComponentCreator<ArrangementCell, ArrangementCell.Data>()
                  .SetData(new ArrangementCell.Data()
                  {
                      IListener = this,
                      
                      Id = data.Id,
                      EMain = Type.EMain.Object,
                      Name = data.Name,
                      Lock = objectInfo == null,
                  })
                  .SetRootRectTm(objectScrollRect.content)
                  .Create();
                
                _arrangementCellList.Add(cell);
            }
        }

        private void ActiveContents()
        {
            UIUtils.SetActive(animalScrollRect?.gameObject, _currETabType == Type.ETab.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, _currETabType == Type.ETab.Object);
        }

        private void Unlock(Type.EMain eMain, int id)
        {
            if (_arrangementCellList == null)
                return;

            foreach (var cell in _arrangementCellList)
            {
                cell?.Unlock(eMain, id);
            }
        }
        
        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return;
            
            Unlock(Type.EMain.Animal, animalInfo.Id);
        }
        
        private void OnChangedObjectInfo(Info.Object objectInfo)
        {
            if (objectInfo == null)
                return;
            
            Unlock(Type.EMain.Object, objectInfo.Id);
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

        #region ArrangementCell.IListener

        void ArrangementCell.IListener.Edit(Type.EMain eMain, int id)
        {
            Deactivate();
            
            Game.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom(eMain == Type.EMain.Animal ? Type.ETab.Animal : Type.ETab.Object);
        }
        #endregion

        public override void Begin()
        {
            base.Begin();

            Debug.Log("Arrangement Popup");

        }
    }
}