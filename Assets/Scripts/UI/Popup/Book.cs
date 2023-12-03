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
    public class Book : BasePopup<Book.Data>, BookCell.IListener, StoryCell.IListener
    {
        public class Data : BaseData
        {
            public int PlaceId = 0;
        }
        
        [SerializeField] private Toggle[] tabToggles = null;
        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;
        [SerializeField] private ScrollRect storyScrollRect = null;

        private Type.ETab _currETabType = Type.ETab.Animal;
        
        private List<BookCell> _bookCellList = new();
        private List<StoryCell> _storyCellList = new();
        private int _placeId = 0;

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
            Game.AnimalManager.Event?.AddListener(OnChangedAnimalInfo);
            ObjectManager.Event?.AddListener(OnChangedObjectInfo);

            _placeId = _data.PlaceId;

            AllDeactiveBookCellList();
            SetAnimalList();
            SetObjectList();
            SetStoryList();
        }

        public override void Activate()
        {
            base.Activate();

            ActivateStoryCellList();

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

        private void ActivateStoryCellList()
        {
            if (_storyCellList == null)
                return;

            foreach(var storyCell in _storyCellList)
            {
                storyCell?.Activate();
            }
        }

        private void AllDeactiveBookCellList()
        {
            if (_bookCellList == null)
                return;

            foreach (var cell in _bookCellList)
            {
                cell?.SetActive(false);
            }
        }

        private BookCell DeactiveBookCell
        {
            get
            {
                foreach (var cell in _bookCellList)
                {
                    if (cell == null)
                        continue;

                    if (cell.gameObject.IsActive())
                        continue;

                    return cell;
                }

                return null;
            }
        }

        private void AddBookCell(int id, Game.Type.EElement eElement, bool isLock, RectTransform rootRectTm)
        {
            BookCell bookCell = DeactiveBookCell;
            var cellData = new BookCell.Data()
            {
                IListener = this,
                Id = id,
                EElement = eElement,
                Lock = isLock,
            };

            if (bookCell != null)
            {
                bookCell.Initialize(cellData);
                bookCell.SetParent(rootRectTm);
                bookCell.SetActive(true);
            }
            else
            {
                bookCell = new ComponentCreator<BookCell, BookCell.Data>()
                    .SetData(cellData)
                    .SetRootRectTm(rootRectTm)
                    .Create();

                _bookCellList.Add(bookCell);
            }
        }

        private void SetAnimalList()
        {
            var dataList = AnimalContainer.Instance?.GetDataListByPlaceId(_placeId);
            if (dataList == null)
                return;

            var animalMgr = MainGameManager.Get<AnimalManager>();
            if (animalMgr == null)
                return;
            
            foreach (var data in dataList)
            {
                var animalInfo = animalMgr.GetAnimalInfo(data.Id);

                AddBookCell(data.Id, Game.Type.EElement.Animal, animalInfo == null, animalScrollRect.content);
            }
        }

        private void SetObjectList()
        {
            var dataList = ObjectContainer.Instance?.GetDataListByPlaceId(_placeId);
            if (dataList == null)
                return;

            var objectMgr = MainGameManager.Get<ObjectManager>();
            if (objectMgr == null)
                return;
            
            foreach (var data in dataList)
            {
                var objectInfo = objectMgr.GetObjectInfoById(data.Id);
                if (objectInfo == null)
                {
                    if (data.EGrade == Type.EObjectGrade.None)
                        continue;
                }

                AddBookCell(data.Id, Game.Type.EElement.Object, objectInfo == null, objectScrollRect.content);
            }
        }

        #region Story
        private void AllDeacitveStoryCellList()
        {
            if (_storyCellList == null)
                return;

            foreach (var cell in _storyCellList)
            {
                cell?.SetActive(false);
            }
        }

        private StoryCell DeactiveStoryCell
        {
            get
            {
                foreach (var cell in _storyCellList)
                {
                    if (cell == null)
                        continue;

                    if (cell.gameObject.IsActive())
                        continue;

                    return cell;
                }

                return null;
            }
        }

        private void AddStoryCell(Story story, int placeId)
        {
            StoryCell storyCell = DeactiveStoryCell;
            var cellData = new StoryCell.Data()
            {
                IListener = this,
                Story = story,
                PlaceId = placeId,
            };

            if (storyCell != null)
            {
                storyCell.Initialize(cellData);
                storyCell.SetActive(true);
            }
            else
            {
                storyCell = new ComponentCreator<StoryCell, StoryCell.Data>()
                   .SetData(cellData)
                   .SetRootRectTm(storyScrollRect?.content)
                   .Create();

                _storyCellList.Add(storyCell);
            }
        }

        private void SetStoryList()
        {
            AllDeacitveStoryCellList();

            int activityPlaceId = GameUtils.ActivityPlaceId;

            var storyList = StoryContainer.Instance.GetStoryList(activityPlaceId);
            if (storyList == null)
                return;

            foreach(var story in storyList)
            {
                if (story == null)
                    continue;

                AddStoryCell(story, activityPlaceId);
            }
        }
        #endregion

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
        
        private void OnChangedObjectInfo(Game.Event.ObjectData objectData)
        {
            if (objectData == null)
                return;

            Unlock(Type.EElement.Object, objectData.Id);
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
        void BookCell.IListener.Click(Game.Type.EElement eElement, int id)
        {
            if (eElement == Type.EElement.Object)
                return;

            if (eElement == Type.EElement.Animal &&
               !MainGameManager.Instance.CheckExist(eElement, id))
                return;

            var popup = new PopupCreator<Profile, Profile.Data>()
                .SetReInitialize(true)
                .SetData(new Profile.Data()
                {
                    EElement = eElement,
                    Id = id,
                })
                .Create();

        }
        #endregion

        #region StoryCell.IListener
        void StoryCell.IListener.Select(Story story)
        {
            MainGameManager.Get<Game.StoryManager>()?.PlayStory(story);
        }
        #endregion
    }
}