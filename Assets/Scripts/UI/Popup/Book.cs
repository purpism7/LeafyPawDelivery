using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using GameSystem;
using Game;
using UI.Component;


namespace UI
{
    public class Book : BasePopup<Book.Data>, BookCell.IListener, StoryCell.IListener, Game.Notification.IListener
    {
        public class Data : BaseData
        {
            public int PlaceId = 0;
        }
        
        [SerializeField]
        private Toggle[] tabToggles = null;
        [SerializeField]
        private RectTransform[] tabRedDotRectTms = null;
        [SerializeField]
        private ScrollRect animalScrollRect = null;
        [SerializeField]
        private ScrollRect objectScrollRect = null;
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
            ObjectManager.Event?.AddListener(OnChangedObject);

            Game.Notification.Get?.AddListener(Notification.EType.AddAnimal, this);
            Game.Notification.Get?.AddListener(Notification.EType.AddObject, this);
            Game.Notification.Get?.AddListener(Notification.EType.AddStory, this);

            _placeId = _data.PlaceId;

            AllDeactiveBookCellList();
            SetAnimalList();
            SetObjectList();
            SetStoryList();

            SetAddAnimal();
            SetAddObject();
            SetAddStory();

            InitializeChildComponent();
        }

        public override void Activate()
        {
            base.Activate();

            ActivateChildComponent(typeof(BookCell));
            ActivateStoryCellList();

            _currETabType = Type.ETab.Animal;
            ActiveContents();

            var tabToggle = tabToggles?.First();
            if (tabToggle != null)
            {
                tabToggle.SetIsOnWithoutNotify(true);
            }

            animalScrollRect?.ResetScrollPos();
            objectScrollRect?.ResetScrollPos();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            Info.Connector.Get?.ResetAddAnimal();
            Info.Connector.Get?.ResetAddObject();
            Info.Connector.Get?.ResetAddStory();
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

            var datas = dataList.OrderBy(obj => obj.Order);

            foreach (var data in datas)
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

        private void AddStoryCell(Story story)
        {
            StoryCell storyCell = DeactiveStoryCell;
            var cellData = new StoryCell.Data()
            {
                IListener = this,
                Story = story,
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

            var storyList = StoryContainer.Instance.GetStoryList(_placeId);
            if (storyList == null)
                return;

            foreach(var story in storyList)
            {
                if (story == null)
                    continue;

                AddStoryCell(story);
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

        private void SetAddAnimal()
        {
            var connector = Info.Connector.Get;
            if (connector == null)
                return;

            var redDotRectTm = tabRedDotRectTms[(int)Game.Type.ETab.Animal];
            if (redDotRectTm)
            {
                UIUtils.SetActive(redDotRectTm, connector.AddAnimalId > 0);
            }
        }

        private void SetAddObject()
        {
            var connector = Info.Connector.Get;
            if (connector == null)
                return;

            var redDotRectTm = tabRedDotRectTms[(int)Game.Type.ETab.Object];
            if (redDotRectTm)
            {
                UIUtils.SetActive(redDotRectTm, connector.AddObjectId > 0);
            }
        }

        private void SetAddStory()
        {
            var connector = Info.Connector.Get;
            if (connector == null)
                return;

            var redDotRectTm = tabRedDotRectTms[(int)Game.Type.ETab.Story];
            if (redDotRectTm)
            {
                UIUtils.SetActive(redDotRectTm, connector.AddStoryId > 0);
            }
        }

        // 도감 오픈 오픈 시, Delay 후 RedDot 안 보이게 처리.
        //private void DelayResetAddAnimal()
        //{
        //    Sequence sequence = DOTween.Sequence()
        //        .SetAutoKill(false)
        //        .AppendInterval(1f)
        //        .OnComplete(() =>
        //        {
        //            Info.Connector.Get?.ResetAddAnimal();
        //        });
        //    sequence.Restart();
        //}

        private void OnChangedAnimalInfo(Game.Event.AnimalData animalData)
        {
            if (animalData == null)
                return;
            
            Unlock(Type.EElement.Animal, animalData.id);
        }
        
        private void OnChangedObject(Game.Event.ObjectData objectData)
        {
            if (objectData == null)
                return;

            Unlock(Type.EElement.Object, objectData.id);
        }

        public void OnChanged(string tabType)
        {
            if(System.Enum.TryParse(tabType, out Type.ETab eTabType))
            {
                if(_currETabType == eTabType)
                    return;

                _currETabType = eTabType;

                ActiveContents();

                EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);
            }
        }

        #region BookCell.IListener
        void BookCell.IListener.Click(Game.Type.EElement eElement, int id)
        {
            //if (eElement == Type.EElement.Object)
            //    return;

            if (!MainGameManager.Instance.CheckExist(eElement, id))
                return;

            var popup = new PopupCreator<Profile, Profile.Data>()
                .SetReInitialize(true)
                .SetData(new Profile.Data()
                {
                    EElement = eElement,
                    Id = id,
                })
                .Create();

            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

        }
        #endregion

        #region StoryCell.IListener
        void StoryCell.IListener.Select(Story story)
        {
            Sequencer.EnqueueTask(() =>
            {
                return MainGameManager.Get<Game.StoryManager>()?.PlayStory(story);
            });

            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);
        }
        #endregion

        #region Game.Notification.IListener
        void Game.Notification.IListener.Notify()
        {
            SetAddAnimal();
            SetAddObject();
            SetAddStory();
        }
        #endregion
    }
}