﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameSystem;
using UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Book : BasePopup<Book.Data>, BookCell.IListener
    {
        public class Data : BaseData
        {

        }
        
        [SerializeField] private Toggle[] tabToggles = null;
        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;

        private Type.ETab _currETabType = Type.ETab.Animal;
        
        private List<BookCell> _bookCellList = new();

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            InternalInit();
        }
        
        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            InternalInit();

            yield return null;
        }

        private void InternalInit()
        {
            MainGameManager.Instance?.AnimalMgr?.Listener?.AddListener(OnChangedAnimalInfo);
            MainGameManager.Instance?.ObjectMgr?.Listener?.AddListener(OnChangedObjectInfo);
            
            _bookCellList.Clear();
            
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
                var animalInfo = animalMgr.GetAnimalInfo(data.Id);
                
                var cell = new ComponentCreator<BookCell, BookCell.Data>()
                    .SetData(new BookCell.Data()
                    {
                        IListener = this,
                        
                        Id = data.Id,
                        EElement = Type.EElement.Animal,
                        Name = data.Name,
                        Lock = animalInfo == null,
                    })
                    .SetRootRectTm(animalScrollRect?.content)
                    .Create();
                
                _bookCellList.Add(cell);
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
                var objectInfo = objectMgr.GetObjectInfoById(data.Id);
                
                var cell = new ComponentCreator<BookCell, BookCell.Data>()
                    .SetData(new BookCell.Data()
                    {
                        IListener = this,
                        
                        Id = data.Id,
                        EElement = Type.EElement.Object,
                        Name = data.Name,
                        Lock = objectInfo == null,
                    })
                    .SetRootRectTm(objectScrollRect?.content)
                    .Create();
                
                _bookCellList.Add(cell);
            }
        }

        private void ActiveContents()
        {
            UIUtils.SetActive(animalScrollRect?.gameObject, _currETabType == Type.ETab.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, _currETabType == Type.ETab.Object);
        }

        private void Unlock(Type.EElement EElement, int id)
        {
            if (_bookCellList == null)
                return;

            foreach (var cell in _bookCellList)
            {
                cell?.Unlock(EElement, id);
            }
        }
        
        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return;
            
            Unlock(Type.EElement.Animal, animalInfo.Id);
        }
        
        private void OnChangedObjectInfo(Info.Object objectInfo)
        {
            if (objectInfo == null)
                return;
            
            Unlock(Type.EElement.Object, objectInfo.Id);
        }

        public void OnChanged(string tabType)
        {
            if(System.Enum.TryParse(tabType, out Type.ETab eTabType))
            {
                if(_currETabType == eTabType)
                    return;

                _currETabType = eTabType;

                ActiveContents();
            }
        }

        #region BookCell.IListener
        void BookCell.IListener.Click()
        {

        }
        #endregion
    }
}