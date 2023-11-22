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

        [SerializeField] private Toggle[] tabToggles = null;
        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;

        private Game.Type.ETab _currETabType = Game.Type.ETab.Animal;

        private List<ArrangementCell> _arrangementCellList = new();
        private int _placeId = 0;

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            InitializeListener();

            _placeId = data.PlaceId;

            AllDeactiveArrangementCellList();
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

        private void InitializeListener()
        {
            Game.AnimalManager.Listener?.RemoveListener(OnChangedAnimalInfo);
            Game.AnimalManager.Listener?.AddListener(OnChangedAnimalInfo);

            Game.ObjectManager.Listener?.AddListener(OnChangedObjectInfo);
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

            foreach (var data in dataList)
            {
                if (data == null)
                    continue;

                var objectInfo = objectMgr.GetObjectInfoById(data.Id);
                if (objectInfo == null)
                {
                    if (data.EGrade == Game.Type.EObjectGrade.None)
                        continue;
                }

                AddArrangementCell(
                    new ArrangementCell.Data()
                    {
                        IListener = this,
                        Id = data.Id,
                        EElement = Game.Type.EElement.Object,
                        Owned = objectInfo != null,
                        Lock = !objectOpenConditionContainer.CheckReq(data.Id),
                    }, objectScrollRect.content);
            }
        }

        private void ActiveContents()
        {
            UIUtils.SetActive(animalScrollRect?.gameObject, _currETabType == Game.Type.ETab.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, _currETabType == Game.Type.ETab.Object);
        }

        private void Obtain(Game.Type.EElement EElement, int id)
        {
            if (_arrangementCellList == null)
                return;

            foreach (var cell in _arrangementCellList)
            {
                cell?.Obtain(EElement, id);

            }
        }
        
        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return;

            Obtain(Game.Type.EElement.Animal, animalInfo.Id);
        }
        
        private void OnChangedObjectInfo(Game.Event.ObjectData objectData)
        {
            if (objectData == null)
                return;

            Obtain(Game.Type.EElement.Object, objectData.Id);

            Debug.Log("OnChangedObjectInfo = " + objectData.Id);


        }

        // 탭 변경 콜백.
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