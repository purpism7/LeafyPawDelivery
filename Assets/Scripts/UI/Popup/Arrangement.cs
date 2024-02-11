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
            public int PlaceId = 0;
        }

        //[SerializeField]
        //private Toggle[] tabToggles = null;
        [SerializeField]
        private Toggle animalToggle = null;
        [SerializeField]
        private Toggle objectToggle = null;

        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;

        private Game.Type.ETab _currETabType = Game.Type.ETab.Animal;

        private List<ArrangementCell> _arrangementCellList = new();
        private int _placeId = 0;
        private int _objectIndex = -1;
        private bool _isTutorial = false;

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            InitializeListener();

            _placeId = data.PlaceId;

            AllDeactiveArrangementCellList();
            SetAnimalList();
            SetObjectList();

            _isTutorial = CheckIsTutorial;
            if (_isTutorial)
            {
                EnableToggle(false);
            }
        }

        public override void Activate()
        {
            base.Activate();

            ActivateArrangementCellList();

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

            if(_isTutorial != isTutorial)
            {
                EnableToggle(true);
                EnableScrollRect(animalScrollRect, true);
                EnableScrollRect(objectScrollRect, true);

                _isTutorial = isTutorial;
            }

            ActiveContents();

            animalToggle?.SetIsOnWithoutNotify(_currETabType == Game.Type.ETab.Animal);
            objectToggle?.SetIsOnWithoutNotify(_currETabType == Game.Type.ETab.Object);

            animalScrollRect?.ResetScrollPos();
            objectScrollRect?.ResetScrollPos();
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

                if(cell.EElement == Game.Type.EElement.Object)
                {
                    int resIndex = GetIndex(objectMgr, cell.Id, ref index);

                    cell.SetIndex(resIndex);
                }

                if(_isTutorial != isTutorial)
                {
                    cell.SetIsTutorial(isTutorial);
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

        //private void AddArrangementCell(int id, Game.Type.EElement eElement, bool isLock, RectTransform rootRectTm)
        //{
        //    ArrangementCell arrangementCell = DeactiveArrangementCell;
        //    var cellData = new ArrangementCell.Data()
        //    {
        //        IListener = this,
        //        Id = id,
        //        EElement = eElement,
        //        Lock = isLock,
        //    };

        //    if (arrangementCell != null)
        //    {
        //        arrangementCell.Initialize(cellData);
        //        arrangementCell.SetParent(rootRectTm);
        //        arrangementCell.SetActive(true);
        //    }
        //    else
        //    {
        //        arrangementCell = new ComponentCreator<ArrangementCell, ArrangementCell.Data>()
        //            .SetData(cellData)
        //            .SetRootRectTm(rootRectTm)
        //            .Create();

        //        _arrangementCellList.Add(arrangementCell);
        //    }
        //}

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

            foreach (var data in dataList)
            {
                if (data == null)
                    continue;

                var animalInfo = animalMgr.GetAnimalInfo(data.Id);

                AddArrangementCell(
                    new ArrangementCell.Data()
                    {
                        IListener = this,
                        Id = data.Id,
                        EElement = Game.Type.EElement.Animal,
                        Owned = animalInfo != null,
                        Lock = !animalOpenConditionContainer.CheckReq(data.Id),
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

        private void Obtain(Game.Type.EElement EElement, int id)
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

                if(cell.Obtain(EElement, id))
                {
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

        private void OnChangedAnimalInfo(Game.Event.AnimalData animalData)
        {
            if (animalData == null)
                return;

            Obtain(Game.Type.EElement.Animal, animalData.id);
        }
        
        private void OnChangedObjectInfo(Game.Event.ObjectData objectData)
        {
            if (objectData == null)
                return;

            Obtain(Game.Type.EElement.Object, objectData.id);
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
                        mainGameMgr?.AddAnimalToPlace(id);
                    });
            }
            else if (EElement == Game.Type.EElement.Object)
            {
                var eTab = EElement == Game.Type.EElement.Animal ? Game.Type.ETab.Animal : Game.Type.ETab.Object;
                Game.UIManager.Instance?.Bottom?.ActivateEditListAfterDeactivateBottom(eTab, index);
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