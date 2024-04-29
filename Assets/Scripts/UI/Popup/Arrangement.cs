using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using Cysharp.Threading.Tasks;

using GameSystem;
using UI.Component;

namespace UI
{
    public class Arrangement : BasePopup<Arrangement.Data>, ArrangementCell.IListener, Game.Notification.IListener
    {
        public class Data : BaseData
        {
            public int PlaceId = 0;
        }

        [SerializeField]
        private Toggle animalToggle = null;
        [SerializeField]
        private Toggle objectToggle = null;
        [SerializeField]
        private RectTransform[] tabRedDotRectTms = null;

        [SerializeField]
        private ScrollRect animalScrollRect = null;
        [SerializeField]
        private GridLayoutGroup animalGridLayoutGroup = null;
        [SerializeField]
        private ScrollRect objectScrollRect = null;
        [SerializeField]
        private GridLayoutGroup objectGridLayoutGroup = null;

        private Game.Type.ETab _currETabType = Game.Type.ETab.Animal;

        private List<ArrangementCell> _arrangementCellList = new();
        private int _placeId = 0;
        private int _objectIndex = -1;
        private bool _isTutorial = false;
        private bool[] _initActivateTab = { false, false };

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            InitializeListener();

            _placeId = data.PlaceId;

            AllDeactiveArrangementCellList();
            SetAnimalList();
            SetObjectList();

            SetNotificationPossibleBuyAnimal();
            SetNotificationPossibleBuyObject();

            _isTutorial = CheckIsTutorial;
            if (_isTutorial)
            {
                EnableToggle(false);
            }
        }

        public override void Activate()
        {
            base.Activate();

            if (_initActivateTab != null)
            {
                _initActivateTab[(int)Game.Type.ETab.Animal] = false;
                _initActivateTab[(int)Game.Type.ETab.Object] = false;
            }

            _currETabType = Game.Type.ETab.Animal;

            var isTutorial = CheckIsTutorial;
            if (isTutorial)
            {
                var tutorialMgr = MainGameManager.Instance?.TutorialMgr;
                if(tutorialMgr != null)
                {
                    if(tutorialMgr.ETutorialStep == Game.Type.ETutorialStep.EditObject)
                    {
                        _currETabType = Game.Type.ETab.Object;
                    }
                }
            }

            animalToggle?.SetIsOnWithoutNotify(_currETabType == Game.Type.ETab.Animal);
            objectToggle?.SetIsOnWithoutNotify(_currETabType == Game.Type.ETab.Object);

            animalScrollRect?.ResetScrollPos();
            objectScrollRect?.ResetScrollPos();

            ActiveContents();

            // 튜토리얼 종료시, 활성화.
            if (_isTutorial != isTutorial)
            {
                EnableToggle(true);
                EnableScrollRect(animalScrollRect, true);
                EnableScrollRect(objectScrollRect, true);

                _isTutorial = isTutorial;
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

        private void InitializeListener()
        {
            Game.AnimalManager.Event?.RemoveListener(OnChangedAnimalInfo);
            Game.AnimalManager.Event?.AddListener(OnChangedAnimalInfo);

            Game.ObjectManager.Event?.RemoveListener(OnChangedObjectInfo);
            Game.ObjectManager.Event?.AddListener(OnChangedObjectInfo);

            Game.Notification.Get?.AddListener(Game.Notification.EType.PossibleBuyAnimal, this);
            Game.Notification.Get?.AddListener(Game.Notification.EType.PossibleBuyObject, this);
        }

        private void ActivateArrangementCellList()
        {
            if (_arrangementCellList == null)
                return;

            var objectMgr = MainGameManager.Get<Game.ObjectManager>();
            if (objectMgr == null)
                return;

            int index = -1;

            var isTutorial = CheckIsTutorial;

            foreach (var cell in _arrangementCellList)
            {
                if (cell == null)
                    continue;

                if (_isTutorial != isTutorial)
                {
                    cell.SetIsTutorial(isTutorial);
                }

                if (cell.EElement == Game.Type.EElement.Animal)
                {
                    if (_currETabType != Game.Type.ETab.Animal)
                        continue;
                }
                else if(cell.EElement == Game.Type.EElement.Object)
                {
                    if (_currETabType != Game.Type.ETab.Object)
                        continue;

                    int resIndex = GetIndex(objectMgr, cell.Id, ref index);

                    cell.SetIndex(resIndex);
                }

                cell.Activate();
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

        private void AllDeactiveArrangementCellList()
        {
            if (_arrangementCellList == null)
                return;

            foreach(var cell in _arrangementCellList)
            {
                cell?.SetActive(false);
            }
        }

        private ArrangementCell DeactiveArrangementCell
        {
            get
            {
                foreach (var cell in _arrangementCellList)
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

        private void AddArrangementCell(ArrangementCell.Data cellData, RectTransform rootRectTm)
        {
            ArrangementCell arrangementCell = DeactiveArrangementCell;

            if (arrangementCell != null)
            {
                arrangementCell.Initialize(cellData);
                arrangementCell.SetParent(rootRectTm);
                arrangementCell.SetActive(true);
            }
            else
            {
                arrangementCell = new ComponentCreator<ArrangementCell, ArrangementCell.Data>()
                    .SetData(cellData)
                    .SetRootRectTm(rootRectTm)
                    .Create();

                _arrangementCellList.Add(arrangementCell);
            }
        }

        private void SetAnimalList()
        {
            var dataList = AnimalContainer.Instance?.GetDataListByPlaceId(_placeId);
            if (dataList == null)
                return;

            var animalMgr = MainGameManager.Get<Game.AnimalManager>();
            if (animalMgr == null)
                return;

            var animalOpenConditionContainer = AnimalOpenConditionContainer.Instance;
            if (animalOpenConditionContainer == null)
                return;

            bool isTutorial = CheckIsTutorial;

            EnableScrollRect(animalScrollRect, !isTutorial);

            for(int i = 0; i < dataList.Count; ++i)
            {
                var data = dataList[i];
                if (data == null)
                    continue;

                var animalInfo = animalMgr.GetAnimalInfo(data.Id);
                bool isLock = !animalOpenConditionContainer.CheckReq(data.Id);
                
                AddArrangementCell(
                    new ArrangementCell.Data()
                    {
                        IListener = this,
                        Id = data.Id,
                        EElement = Game.Type.EElement.Animal,
                        Owned = animalInfo != null,
                        Lock = isLock,
                        isTutorial = isTutorial,
                    }, animalScrollRect.content);
            }
        }

        private void SetObjectList()
        {
            var dataList = ObjectContainer.Instance?.GetDataListByPlaceId(_placeId);
            if (dataList == null)
                return;

            var objectMgr = MainGameManager.Get<Game.ObjectManager>();
            if (objectMgr == null)
                return;

            var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
            if (objectOpenConditionContainer == null)
                return;

            _objectIndex = -1;
            bool isTutorial = CheckIsTutorial;

            EnableScrollRect(objectScrollRect, !isTutorial);

            var datas = dataList.OrderBy(obj => obj.Order);

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

                int resIndex = GetIndex(objectMgr, data.Id, ref _objectIndex);
                
                AddArrangementCell(
                    new ArrangementCell.Data()
                    {
                        IListener = this,
                        Id = data.Id,
                        EElement = Game.Type.EElement.Object,
                        Owned = objectInfo != null,
                        Lock = !objectOpenConditionContainer.CheckReq(data.Id),
                        isTutorial = isTutorial,

                        index = resIndex,
                    }, objectScrollRect.content);
            }
        }

        private void ActiveContents()
        {
            UIUtils.SetActive(animalScrollRect?.gameObject, _currETabType == Game.Type.ETab.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, _currETabType == Game.Type.ETab.Object);

            ActivateArrangementCellList();

            if (_initActivateTab != null &&
               !_initActivateTab[(int)_currETabType])
            {
                _initActivateTab[(int)_currETabType] = true;

                MoveScrollPossibleBuy().Forget();
            }
        }

        private void EnableToggle(bool enable)
        {
            if (animalToggle != null)
            {
                animalToggle.enabled = enable;
            }

            if(objectToggle != null)
            {
                objectToggle.enabled = enable;
            }
        }

        private void EnableScrollRect(ScrollRect scrollRect, bool enable)
        {
            if (scrollRect == null)
                return;

            scrollRect.enabled = enable;
        }

        private int GetIndex(Game.ObjectManager objectMgr, int id, ref int index)
        {
            if (objectMgr == null)
                return -1;

            if(objectMgr.GetRemainCount(id) > 0)
            {
                return ++index;
            }
            else
            {
                return -1;
            }
        }

        private void Obtain(Game.Type.EElement eElement, int id)
        {
            var objectMgr = MainGameManager.Get<Game.ObjectManager>();
            if (objectMgr == null)
                return;

            if (_arrangementCellList == null)
                return;

            foreach (var cell in _arrangementCellList)
            {
                if (cell == null)
                    continue;

                if(cell.Obtain(eElement, id))
                {
                    Info.Connector.Get?.ResetPossibleBuyAnimal();
                    Info.Connector.Get?.ResetPossibleBuyObject();

                    cell.SetIndex(GetIndex(objectMgr, cell.Id, ref _objectIndex));
                }
            }
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

        private int GetArrangementCellIndex(Game.Type.EElement eElement, int id)
        {
            if (id <= 0)
                return -1;

            var sortArrangementCellList = _arrangementCellList?.OrderBy(cell => cell.Id)?.ToList();
            if (sortArrangementCellList == null)
                return -1;

            int index = 0;

            for (int i = 0; i < sortArrangementCellList.Count ; ++i)
            {
                var cell = _arrangementCellList[i];
                if (cell == null)
                    continue;

                if (cell.EElement != eElement)
                    continue;

                if (cell.Id == id)
                {
                    return index;
                }

                ++index;
            }

            return index;
        }

        private async UniTask MoveScrollPossibleBuy()
        {
            if(_currETabType == Game.Type.ETab.Animal)
            {
                int index = GetArrangementCellIndex(Game.Type.EElement.Animal, PossibleBuyAnimal);
                if (index < 0)
                    return;

                if (animalGridLayoutGroup != null)
                {
                    await UniTask.Yield();

                    animalScrollRect?.MoveVerticalScrollToIndex(animalGridLayoutGroup.cellSize.y, index, false);
                }

                //Debug.Log("index = " + index);
            }
            else if(_currETabType == Game.Type.ETab.Object)
            {
                int index = GetArrangementCellIndex(Game.Type.EElement.Object, PossibleBuyObject);
                if (index < 0)
                    return;

                if(objectGridLayoutGroup != null)
                {
                    await UniTask.Yield();

                    objectScrollRect?.MoveVerticalScrollToIndex(objectGridLayoutGroup.cellSize.y, index, true);
                }
            }
        }

        #region Notification
        private void SetNotificationPossibleBuyAnimal()
        {
            var redDotRectTm = tabRedDotRectTms[(int)Game.Type.ETab.Animal];
            if (redDotRectTm)
            {
                UIUtils.SetActive(redDotRectTm, PossibleBuyAnimal > 0);
            }
        }

        private void SetNotificationPossibleBuyObject()
        {
            var redDotRectTm = tabRedDotRectTms[(int)Game.Type.ETab.Object];
            if (redDotRectTm)
            {
                UIUtils.SetActive(redDotRectTm, PossibleBuyObject > 0);
            }
        }

        private int PossibleBuyAnimal
        {
            get
            {
                var connector = Info.Connector.Get;
                if (connector == null)
                    return 0;

                return connector.PossibleBuyAnimal;
            }
        }

        private int PossibleBuyObject
        {
            get
            {
                var connector = Info.Connector.Get;
                if (connector == null)
                    return 0;

                return connector.PossibleBuyObject;
            }
        }
        #endregion

        private void OnChangedAnimalInfo(Game.Event.AnimalData animalData)
        {
            if (animalData == null)
                return;

            if(animalData is Game.Event.AddAnimalData)
            {
                Obtain(Game.Type.EElement.Animal, animalData.id);

                Info.Connector.Get?.SetPossibleBuyObject();
            }
        }
        
        private void OnChangedObjectInfo(Game.Event.ObjectData objectData)
        {
            if (objectData == null)
                return;

            if(objectData is Game.Event.AddObjectData)
            {
                bool isHiddenObject = (objectData as Game.Event.AddObjectData).eOpenConditionType == OpenConditionData.EType.Hidden;

                Obtain(Game.Type.EElement.Object, objectData.id);

                if(isHiddenObject)
                {
                    Info.Connector.Get?.SetPossibleBuyObject();
                }

                Info.Connector.Get?.SetPossibleBuyAnimal();
            }
        }

        // 탭 변경 콜백.
        public void OnChanged(string tabType)
        {
            if (CheckIsTutorial)
                return;

            if(System.Enum.TryParse(tabType, out Game.Type.ETab eTabType))
            {
                if(_currETabType == eTabType)
                    return;

                _currETabType = eTabType;

                ActiveContents();

                EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);
            }
        }

        #region ArrangementCell.IListener
        void ArrangementCell.IListener.Edit(Game.Type.EElement EElement, int id, int index)
        {
            Deactivate();

            if (EElement == Game.Type.EElement.Animal)
            {
                var mainGameMgr = MainGameManager.Instance;

                mainGameMgr?.SetGameStateAsync(Game.Type.EGameState.Edit);

                Game.UIManager.Instance?.Bottom?.DeactivateAnim(
                    () =>
                    {
                        mainGameMgr?.SpwanAnimalToPlace(id, true);
                    });
            }
            else if (EElement == Game.Type.EElement.Object)
            {
                var eTab = EElement == Game.Type.EElement.Animal ? Game.Type.ETab.Animal : Game.Type.ETab.Object;
                Game.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom(eTab, index);
            }
        }
        #endregion

        #region Game.Notification.IListener
        void Game.Notification.IListener.Notify()
        {
            SetNotificationPossibleBuyAnimal();
            SetNotificationPossibleBuyObject();
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