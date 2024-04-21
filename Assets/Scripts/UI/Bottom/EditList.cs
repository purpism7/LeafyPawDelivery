using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using Cysharp.Threading.Tasks;

using Game;
using UI.Component;

namespace UI
{
    public class EditList : Base<EditList.Data>, EditAnimal.IListener, EditObject.IListener, ResetArrange.IListener
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
        }

        public interface IListener
        {
            void Close();
        }

        [SerializeField]
        private ScrollRect animalScrollRect = null;
        [SerializeField]
        private ScrollRect objectScrollRect = null;
        [SerializeField]
        private Toggle animalToggle = null;
        [SerializeField]
        private Toggle objectToggle = null;
        [SerializeField]
        private Button resetBtn = null;

        private List<Component.EditAnimal> _editAnimalList = new();
        private List<Component.EditObject> _editObjectList = new();

        public Type.ETab CurrETabType { get; private set; } = Type.ETab.Animal;

        private bool _editing = false;
        private int _selectIndex = -1;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _editObjectList?.Clear();
            _editAnimalList?.Clear();
        }

        public override void Activate()
        {
            base.Activate();
            
            ActiveContents();

            SetAnimalList();
            SetObjectList();

            EnableToggleAndBtn(!CheckIsTutorial);

            if(CurrETabType == Type.ETab.Object)
            {
                MoveScrollToIndex(objectScrollRect, _selectIndex).Forget();
            }

            _editing = false;
        }

        public EditList Setup(Type.ETab eTabType, int index = -1)
        {
            if (CheckIsTutorial)
            {
                var tutorialMgr = MainGameManager.Instance?.TutorialMgr;
                if (tutorialMgr != null)
                {
                    if (tutorialMgr.ETutorialStep == Game.Type.ETutorialStep.EditObject)
                    {
                        eTabType = Game.Type.ETab.Object;
                    }
                }
            }

            SetTab(eTabType);

            animalToggle?.SetIsOnWithoutNotify(eTabType == Type.ETab.Animal);
            objectToggle?.SetIsOnWithoutNotify(eTabType == Type.ETab.Object);

            _selectIndex = index;

            return this;
        }

        private async UniTask MoveScrollToIndex(ScrollRect scrollRect, int index)
        {
            if (index < 0)
                return;

            var gridLayoutGroup = scrollRect?.content?.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup != null)
            {
                await UniTask.Yield();

                scrollRect?.MoveHorizontalScrollToIndex(gridLayoutGroup.cellSize.x, index);
            }
        }

        private void SetAnimalList()
        {
            DeactviateAllAnimal();

            var infoList = MainGameManager.Get<AnimalManager>()?.AnimalInfoList;
            if(infoList == null)
                return;

            int placeId = GameUtils.ActivityPlaceId;
            bool isTutorial = CheckIsTutorial;

            EnableScrollRect(animalScrollRect, !isTutorial);

            foreach (var info in infoList)
            {
                if(info == null)
                    continue;

                var animalData = AnimalContainer.Instance?.GetData(info.Id);
                if (animalData == null)
                    continue;

                if (animalData.PlaceId != placeId)
                    continue;

                if (info.Arrangement)
                    continue;

                var data = new Component.EditAnimal.Data()
                {
                    iListener = this,
                    AnimalData = animalData,
                    isTutorial = isTutorial,
                };

                CreateEditAnimal(data);
            }
        }
        
        private void SetObjectList()
        {
            DeactviateAllObject(); 

            var objectMgr = MainGameManager.Get<ObjectManager>();
            if (objectMgr == null)
                return;

            int placeId = GameUtils.ActivityPlaceId;
            var objectDataList = ObjectContainer.Instance?.GetDataListByPlaceId(placeId);

            var objectDatas = objectDataList?.OrderBy(data => data.Id);

            bool isTutorial = CheckIsTutorial;
            EnableScrollRect(objectScrollRect, !isTutorial);

            foreach(var objectData in objectDatas)
            {
                if (objectData == null)
                    continue;

                int reaminCount = objectMgr.GetRemainCount(objectData.Id);
                if (reaminCount <= 0)
                    continue;

                var data = new Component.EditObject.Data()
                {
                    iListener = this,
                    ObjectId = objectData.Id,
                    Count = objectData.Count,
                    RemainCount = reaminCount,
                    isTutorial = isTutorial,
                };

                CreateEditObject(data);
            }
        }

        private void CreateEditAnimal(Component.EditAnimal.Data data)
        {
            if (data == null)
                return;

            foreach(var editAnimal in _editAnimalList)
            {
                if (editAnimal == null)
                    continue;

                if (editAnimal.gameObject.IsActive())
                    continue;

                editAnimal.Initialize(data);
                editAnimal.gameObject?.SetActive(true);

                return;
            }

            var addEditAnimal = new GameSystem.UICreator<UI.Component.EditAnimal, UI.Component.EditAnimal.Data>()
                      .SetData(data)
                      .SetRooTm(animalScrollRect.content)
                      .Create();

            _editAnimalList.Add(addEditAnimal);
        }

        private void CreateEditObject(Component.EditObject.Data data)
        {
            if (data == null)
                return;

            foreach (var editObject in _editObjectList)
            {
                if (editObject == null)
                    continue;

                if (editObject.gameObject.IsActive())
                    continue;

                editObject.Initialize(data);
                editObject.gameObject?.SetActive(true);

                return;
            }

            var addEditObject = new GameSystem.UICreator<UI.Component.EditObject, UI.Component.EditObject.Data>()
                   .SetData(data)
                   .SetRooTm(objectScrollRect.content)
                   .Create();

            _editObjectList.Add(addEditObject);
        }
        
        private void ActiveContents()
        {
            UIUtils.SetActive(animalScrollRect?.gameObject, CurrETabType == Type.ETab.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, CurrETabType == Type.ETab.Object);
        }

        private void DeactviateAllAnimal()
        {
            foreach (var animal in _editAnimalList)
            {
                animal?.gameObject.SetActive(false);
            }
        }

        private void DeactviateAllObject()
        {
            foreach(var obj in _editObjectList)
            {
                obj?.gameObject.SetActive(false);
            }
        }

        public void RefreshAnimalList()
        {
            var animalMgr = MainGameManager.Get<AnimalManager>();
            if (animalMgr == null)
                return;

            var infoList = animalMgr.AnimalInfoList;
            if (infoList == null)
                return;

            DeactviateAllAnimal();

            int placeId = GameUtils.ActivityPlaceId;

            for (int i = 0; i < infoList.Count; ++i)
            {
                var info = infoList[i];
                if (info == null)
                    continue;

                var animalData = AnimalContainer.Instance.GetData(info.Id);
                if (animalData == null)
                    continue;

                if (animalData.PlaceId != placeId)
                    continue;

                if (info.Arrangement)
                    continue;

                var data = new Component.EditAnimal.Data()
                {
                    iListener = this,
                    AnimalData = animalData,
                };

                if (_editAnimalList?.Count > i)
                {
                    var editAnimal = _editAnimalList[i];
                    if (editAnimal != null)
                    {
                        editAnimal.Initialize(data);
                        editAnimal.gameObject.SetActive(true);
                    }
                }
                else
                {
                    CreateEditAnimal(data);
                }
            }
        }

        public void RefreshObjectList()
        {
            var objectMgr = MainGameManager.Get<ObjectManager>();
            if (objectMgr == null)
                return;

            var infoList = objectMgr.ObjectInfoList;
            if (infoList == null)
                return;

            DeactviateAllObject();

            int placeId = GameUtils.ActivityPlaceId;

            for (int i = 0; i < infoList.Count; ++i)
            {
                var objectInfo = infoList[i];
                if (objectInfo == null)
                    continue;

                var objectData = ObjectContainer.Instance.GetData(objectInfo.Id);
                if (objectData == null)
                    continue;

                if (objectData.PlaceId != placeId)
                    continue;

                int remainCount = objectMgr.GetRemainCount(objectInfo.Id);
                if (remainCount <= 0)
                    continue;

                var data = new Component.EditObject.Data()
                {
                    iListener = this,
                    ObjectId = objectInfo.Id,
                    Count = objectData.Count,
                    RemainCount = remainCount,
                };

                if(_editObjectList?.Count > i)
                {
                    var editObj = _editObjectList[i];
                    if (editObj != null)
                    {
                        editObj.Initialize(data);
                        editObj.gameObject.SetActive(true);
                    }
                }
                else
                {
                    CreateEditObject(data);
                }
            }
        }

        private void SetTab(Type.ETab eTabType)
        {
            CurrETabType = eTabType;

            ActiveContents();
        }

        private void EnableToggleAndBtn(bool enable)
        {
            if (animalToggle != null)
            {
                animalToggle.enabled = enable;
            }

            if (objectToggle != null)
            {
                objectToggle.enabled = enable;
            }

            if(resetBtn != null)
            {
                resetBtn.interactable = enable;
            }
        }

        private void EnableScrollRect(ScrollRect scrollRect, bool enable)
        {
            if (scrollRect == null)
                return;

            scrollRect.enabled = enable;
        }

        private bool CheckIsTutorial
        {
            get
            {
                var mainGameMgr = MainGameManager.Instance;
                if (mainGameMgr != null)
                {
                    return mainGameMgr.IsTutorial;
                }

                return false;
            }
        }

        #region EditAnimal.IListener
        void EditAnimal.IListener.Select(int id)
        {
            if (_editing)
                return;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            mainGameMgr.SpwanAnimalToPlace(id, true);

            _editing = true;

            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);

        }
        #endregion

        #region EditObject.IListener
        void EditObject.IListener.Select(int id)
        {
            if (_editing)
                return;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            mainGameMgr.SpwanObjectToPlace(id);

            _editing = true;

            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
        }
        #endregion

        #region ResetArrange.IListener
        void ResetArrange.IListener.Reset()
        {
            var placeMgr = MainGameManager.Get<Game.PlaceManager>();

            IPlace iPlace = placeMgr?.ActivityPlace;
            iPlace?.RemoveAll(
                () =>
                {
                    RefreshAnimalList();
                    RefreshObjectList();

                    MoveScrollToIndex(animalScrollRect, 0).Forget();
                    MoveScrollToIndex(objectScrollRect, 0).Forget();
                });
        }
        #endregion

        public void OnChanged(string tabType)
        {
            if(System.Enum.TryParse(tabType, out Type.ETab eTabType))
            {
                if(CurrETabType == eTabType)
                    return;
                
                SetTab(eTabType);

                GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
            }
        }

        public void OnClickClose()
        {
            _data?.IListener?.Close();

            GameSystem.EffectPlayer.Get?.Play(GameSystem.EffectPlayer.AudioClipData.EType.TouchButton);
        }

        public void OnClickAllReset()
        {
            Sequencer.EnqueueTask(
                () =>
                {
                    var resetArrange = new GameSystem.PopupCreator<ResetArrange, ResetArrange.Data>()
                        .SetData(new ResetArrange.Data()
                        {
                            iListener = this,
                        })
                        .Create();

                    return resetArrange;
                });
        }
    }
}

