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

        private Game.Type.ETab _currETabType = Game.Type.ETab.Animal;

        private List<ArrangementCell> _arrangementCellList = new();

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));
            
            Game.AnimalManager.Listener?.AddListener(OnChangedAnimalInfo);
            MainGameManager.Instance?.ObjectMgr?.Listener?.AddListener(OnChangedObjectInfo);
            
            _arrangementCellList.Clear();
            
            SetAnimalList();
            SetObjectList();
        }

        public override void Activate()
        {
            base.Activate();

            ActivateArrangementCellList();

            _currETabType = Game.Type.ETab.Animal;
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

            DeactivateArrangementCellList();

            _endTask = true;
        }

        public override void ClickClose()
        {
            base.ClickClose();
        }

        private void ActivateArrangementCellList()
        {
            if (_arrangementCellList == null)
                return;

            foreach(var cell in _arrangementCellList)
            {
                cell?.Activate();
            }
        }

        private void DeactivateArrangementCellList()
        {
            if (_arrangementCellList == null)
                return;

            foreach (var cell in _arrangementCellList)
            {
                cell?.Deactivate();
            }
        }

        private void SetAnimalList()
        {
            var datas = AnimalContainer.Instance?.Datas;
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
                        EElement = Game.Type.EElement.Animal,
                        Lock = animalInfo == null,
                    })
                    .SetRootRectTm(animalScrollRect.content)
                    .Create();
                
                _arrangementCellList.Add(cell);
            }
        }

        private void SetObjectList()
        {
            var datas = ObjectContainer.Instance?.Datas;
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
                if (objectInfo == null)
                {
                    if (data.EGrade == Game.Type.EObjectGrade.None)
                        continue;
                }

                var cell = new ComponentCreator<ArrangementCell, ArrangementCell.Data>()
                  .SetData(new ArrangementCell.Data()
                  {
                      IListener = this,
                      Id = data.Id,
                      EElement = Game.Type.EElement.Object,
                      Lock = objectInfo == null,
                  })
                  .SetRootRectTm(objectScrollRect.content)
                  .Create();
                
                _arrangementCellList.Add(cell);
            }
        }

        private void ActiveContents()
        {
            UIUtils.SetActive(animalScrollRect?.gameObject, _currETabType == Game.Type.ETab.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, _currETabType == Game.Type.ETab.Object);
        }

        private void Unlock(Game.Type.EElement EElement, int id)
        {
            if (_arrangementCellList == null)
                return;

            foreach (var cell in _arrangementCellList)
            {
                cell?.Unlock(EElement, id);
            }
        }
        
        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return;
            
            Unlock(Game.Type.EElement.Animal, animalInfo.Id);
        }
        
        private void OnChangedObjectInfo(int id)
        {
            Unlock(Game.Type.EElement.Object, id);
        }

        public void OnChanged(string tabType)
        {
            if(System.Enum.TryParse(tabType, out Game.Type.ETab eTabType))
            {
                if(_currETabType == eTabType)
                    return;

                _currETabType = eTabType;

                ActiveContents();
            }
        }

        #region ArrangementCell.IListener
        void ArrangementCell.IListener.Edit(Game.Type.EElement EElement, int id)
        {
            Deactivate();

            if (EElement == Game.Type.EElement.Animal)
            {
                Game.UIManager.Instance?.Bottom?.DeactivateBottom(
                    () =>
                    {
                        MainGameManager.Instance?.AddAnimalToPlace(id);
                    });
            }
            else if (EElement == Game.Type.EElement.Object)
            {
                var eTab = EElement == Game.Type.EElement.Animal ? Game.Type.ETab.Animal : Game.Type.ETab.Object;
                Game.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom(eTab);
            }
        }
        #endregion

        public override void Begin()
        {
            base.Begin();

            _endTask = false;
        }

        public override bool End => _endTask;
    }
}