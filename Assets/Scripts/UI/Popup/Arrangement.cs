using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using Cysharp.Threading.Tasks;
using Game;
using GameSystem;
using TMPro;
using UI.Component;
using Unity.VisualScripting;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class Arrangement : BasePopup<Arrangement.Data>, AnimalArrangementCell.IListener, ObjectArrangementCell.IListener, Game.Notification.IListener
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
        
        [Header("Object")]
        [SerializeField]
        private RectTransform objectRootRectTm = null;
        [SerializeField]
        private ScrollRect objectScrollRect = null;
        [SerializeField]
        private GridLayoutGroup objectGridLayoutGroup = null;
        [SerializeField] 
        private ScrollRect specialObjectScrollRect = null;
        [SerializeField]
        private RectTransform[] unselectedRootRectTms = null;
        
        [SerializeField]
        private TextMeshProUGUI getReadyTMP = null;

        private Game.Type.ETab _currETabType = Game.Type.ETab.Animal;

        private List<IArrangementCell> _arrangementCellList = new();
        private int _placeId = 0;
        // private int _objectIndex = -1;
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
                        _currETabType = Game.Type.ETab.Object;
                }
            }

            animalToggle?.SetIsOnWithoutNotify(_currETabType == Game.Type.ETab.Animal);
            objectToggle?.SetIsOnWithoutNotify(_currETabType == Game.Type.ETab.Object);

            animalScrollRect?.ResetScrollPos();
            objectScrollRect?.ResetScrollPos();
            specialObjectScrollRect?.ResetScrollPos();

            ActiveContents();
            ActiveObjectList(0);

            // 튜토리얼 종료시, 활성화.
            if (_isTutorial != isTutorial)
            {
                EnableToggle(true);
                EnableScrollRect(animalScrollRect, true);
                EnableScrollRect(objectScrollRect, true);
                EnableScrollRect(specialObjectScrollRect, true);

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
                    cell.SetIsTutorial(isTutorial);

                if (cell.ElementType == Game.Type.EElement.Animal)
                {
                    if (_currETabType != Game.Type.ETab.Animal)
                        continue;
                }
                else if(cell.ElementType == Game.Type.EElement.Object)
                {
                    if (_currETabType != Game.Type.ETab.Object)
                        continue;
                    
                    cell.SetIndex(GetIndex(objectMgr, cell.Id));
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
                GameUtils.SetActive(cell.Transform, false);
                //cell?.SetActive(false);
            }
        }

        private TCell GetDeactiveArrangementCell<TCell>() where TCell : class
        {
            foreach (var cell in _arrangementCellList)
            {
                if (cell == null)
                    continue;

                if (cell.Transform.IsActive())
                    continue;

                return cell as TCell;
            }

            return null;
        }

        private void AddAnimalArrangementCell(AnimalArrangementCell.Data cellData, RectTransform rootRectTm, int order = 0)
        {
            AnimalArrangementCell arrangementCell = GetDeactiveArrangementCell<AnimalArrangementCell>();
            if (arrangementCell != null)
            {
                arrangementCell.Initialize(cellData);
                arrangementCell.SetParent(rootRectTm);
                arrangementCell.SetActive(true);

                // if (cellData?.EElement == Type.EElement.Object)
                {
                    // if (cellData.IsSpecialObject)
                    {
                        arrangementCell.transform.SetSiblingIndex(order);
                    }
                }
            }
            else
            {
                arrangementCell = new ComponentCreator<AnimalArrangementCell, AnimalArrangementCell.Data>()
                   .SetData(cellData)
                   .SetRootRectTm(rootRectTm)
                   .Create();

                _arrangementCellList.Add(arrangementCell);
            }
        }

        private void AddObjectArrangementCell(ObjectArrangementCell.Data cellData, RectTransform rootRectTm, int order = 0)
        {
            ObjectArrangementCell arrangementCell = GetDeactiveArrangementCell<ObjectArrangementCell>(); 
            if (arrangementCell != null)
            {
                arrangementCell.Initialize(cellData);
                arrangementCell.SetParent(rootRectTm);
                arrangementCell.SetActive(true);

                // if (cellData?.EElement == Type.EElement.Object)
                {
                    // if (cellData.IsSpecialObject)
                    {
                        arrangementCell.transform.SetSiblingIndex(order);
                    }
                }
            }
            else
            {
                arrangementCell = new ComponentCreator<ObjectArrangementCell, ObjectArrangementCell.Data>()
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
                
                AddAnimalArrangementCell(
                    new AnimalArrangementCell.Data
                    {
                        IListener = this,
                        Id = data.Id,
                        EElement = Game.Type.EElement.Animal,
                        Owned = animalInfo != null,
                        Lock = isLock,
                        
                        IsSpecialObject = AnimalContainer.Instance.CheckExistInteraction(data.Id),
                        isTutorial = isTutorial,
                    }, animalScrollRect.content, data.Id);
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

            bool isTutorial = CheckIsTutorial;

            EnableScrollRect(objectScrollRect, !isTutorial);
            EnableScrollRect(specialObjectScrollRect, !isTutorial);

            var datas = dataList.OrderBy(obj => obj.Order);
            var orderDataList = datas.OrderByDescending(obj => obj.Grade == Type.EObjectGrade.Special).ToList();
            
            for(int i = orderDataList.Count - 1; 0 <= i; --i)
            {
                if(orderDataList[i] == null)
                    continue;
                
                if(objectMgr.GetRemainCount(orderDataList[i].Id) > 0)
                    continue;

                orderDataList.Remove(orderDataList[i]);
            }
            
            foreach (var objectData in datas)
            {
                var objectInfo = objectMgr.GetObjectInfoById(objectData.Id);
                if (objectInfo == null)
                {
                    if (objectData.Grade == Game.Type.EObjectGrade.None)
                        continue;
                }

                // int resIndex = -1;
                // // if(data.EGrade != Type.EObjectGrade.Special)
                //     resIndex = GetIndex(objectMgr, data.Id, ref _objectIndex);

                var objectId = objectData.Id;
                int index = orderDataList.FindIndex(obj => obj.Id == objectId);
                
                int count = objectData.Count;
                if (objectData.ObjectType == Type.ObjectType.Garden)
                    count = objectInfo?.Count ?? 0;

                var data = new ObjectArrangementCell.Data
                {
                    IListener = this,
                    Id = objectId,
                    EElement = Game.Type.EElement.Object,
                    Owned = objectInfo != null,
                    Lock = !objectOpenConditionContainer.CheckReq(objectId),
                    isTutorial = isTutorial,
                    IsSpecialObject = objectData.Grade == Type.EObjectGrade.Special,

                    index = index,
                }.WithObjectType(objectData.ObjectType)
                .WithCount(count)
                .WithRemainCount(objectMgr.GetRemainCount(objectId));

                AddObjectArrangementCell(data, objectData.Grade == Type.EObjectGrade.Special ? specialObjectScrollRect.content : objectScrollRect.content, objectData.Order);
            }
        }
        
        private void ActiveContents()
        {
            animalScrollRect?.SetActive(_currETabType == Game.Type.ETab.Animal);
            objectRootRectTm?.SetActive(_currETabType == Game.Type.ETab.Object);
            
            ActivateArrangementCellList();

            if (_initActivateTab != null &&
               !_initActivateTab[(int)_currETabType])
            {
                _initActivateTab[(int)_currETabType] = true;

                MoveScrollPossibleBuy().Forget();
            }
            
            // getReadyTMP?.SetText(string.Empty);
            GameUtils.SetActive(getReadyTMP, false);
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

        private int GetIndex(Game.ObjectManager objectMgr, int id)
        {
            var dataList = ObjectContainer.Instance?.GetDataListByPlaceId(_placeId);
            if (dataList == null)
                return -1;
            
            var orderDataList = dataList.OrderBy(obj => obj.Order).ToList();
            orderDataList = orderDataList.OrderByDescending(obj => obj.Grade == Type.EObjectGrade.Special).ToList();
            
            for(int i = orderDataList.Count - 1; 0 <= i; --i)
            {
                if(orderDataList[i] == null)
                    continue;
                
                if(objectMgr.GetRemainCount(orderDataList[i].Id) > 0)
                    continue;

                orderDataList.Remove(orderDataList[i]);
            }
            
            return orderDataList.FindIndex(obj => obj.Id == id);
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

                    cell.SetIndex(GetIndex(objectMgr, cell.Id));
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
            if (_isTutorial)
                return 0;
            
            if (id <= 0)
                return -1;
            
            var sortArrangementCellList = _arrangementCellList?.OrderBy(cell => cell.Id).ToList();
            if (sortArrangementCellList == null)
                return -1;

            int index = 0;

            for (int i = 0; i < sortArrangementCellList.Count ; ++i)
            {
                var cell = _arrangementCellList[i];
                if (cell == null)
                    continue;

                if (cell.ElementType != eElement)
                    continue;

                if (cell.Id == id)
                    return index;

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
        
        private void ActiveObjectList(int index)
        {
            if (unselectedRootRectTms == null)
                return;

            for (int i = 0; i < unselectedRootRectTms.Length; ++i)
            {
                GameUtils.SetActive(unselectedRootRectTms[i], i != index);
            }
            
            GameUtils.SetActive(objectScrollRect, index == 0);
            GameUtils.SetActive(specialObjectScrollRect, index == 1);

            var local = string.Empty;
            var dataList = ObjectContainer.Instance?.GetDataListByPlaceId(_placeId);
            if (dataList != null)
            {
                var findObj = dataList.Find(obj => obj?.Grade == Type.EObjectGrade.Special);
                if(findObj == null && index == 1)
                    local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "desc_get_ready_so", LocalizationSettings.SelectedLocale);
            }
            
            getReadyTMP?.SetText(local);
            GameUtils.SetActive(getReadyTMP, string.IsNullOrEmpty(local) == false);
        }
        
        public void OnClickObjectTab(int index)
        {
            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);
            
            ActiveObjectList(index);
        }

        #region Notification
        private void SetNotificationPossibleBuyAnimal()
        {
            var redDotRectTm = tabRedDotRectTms[(int)Game.Type.ETab.Animal];
            if (redDotRectTm)
            {
                GameUtils.SetActive(redDotRectTm, PossibleBuyAnimal > 0);
            }
        }

        private void SetNotificationPossibleBuyObject()
        {
            var redDotRectTm = tabRedDotRectTms[(int)Game.Type.ETab.Object];
            if (redDotRectTm)
            {
                GameUtils.SetActive(redDotRectTm, PossibleBuyObject > 0);
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

        #region AnimalArrangementCell.IListener
        void AnimalArrangementCell.IListener.Edit(int id, int index)
        {
            Deactivate();

            var mainGameMgr = MainGameManager.Instance;
            //if (EElement == Game.Type.EElement.Animal)
            {
                mainGameMgr?.SetGameStateAsync(Game.Type.EGameState.Edit);

                Game.UIManager.Instance?.Bottom?.DeactivateAnim(
                    () =>
                    {
                        mainGameMgr?.SpwanAnimalToPlace(id, true);
                    });
            }
            //else if (EElement == Game.Type.EElement.Object)
            //{
            //    var eTab = EElement == Game.Type.EElement.Animal ? Game.Type.ETab.Animal : Game.Type.ETab.Object;
            //    Game.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom(eTab, index);
            //}
        }
        #endregion

        #region ObjectArrangementCell.IListener
        void ObjectArrangementCell.IListener.Edit(int id, int index)
        {
            Deactivate();

            //var eTab = EElement == Game.Type.EElement.Animal ? Game.Type.ETab.Animal : Game.Type.ETab.Object;
            Game.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom(Game.Type.ETab.Object, index);
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