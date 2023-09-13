using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameSystem;

using UnityEngine;
using UnityEngine.UI;

using Game;
using UI.Component;

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
        [SerializeField] private ScrollRect storyScrollRect = null;

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
                if (objectInfo == null)
                {
                    if (data.EGrade == Type.EObjectGrade.None)
                        continue;
                }

                var cell = new ComponentCreator<BookCell, BookCell.Data>()
                    .SetData(new BookCell.Data()
                    {
                        IListener = this,
                        
                        Id = data.Id,
                        EElement = Type.EElement.Object,
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
            UIUtils.SetActive(storyScrollRect?.gameObject, _currETabType == Type.ETab.Story);
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
        
        private void OnChangedObjectInfo(int id)
        {
            Unlock(Type.EElement.Object, id);
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
        void BookCell.IListener.Click(Game.Type.EElement eElment, int id)
        {
            if (eElment == Type.EElement.Object)
                return;

            if (eElment == Type.EElement.Animal &&
               !MainGameManager.Instance.AnimalMgr.CheckExist(id))
                return;

            var popup = new PopupCreator<Profile, Profile.Data>()
                .SetReInitialize(true)
                .SetData(new Profile.Data()
                {
                    EElement = eElment,
                    Id = id,
                })
                .Create();

        }
        #endregion
    }
}