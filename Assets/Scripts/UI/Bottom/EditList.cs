using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;
using UI.Component;

namespace UI
{
    public class EditList : Base<EditList.Data>, EditAnimal.IListener, EditObject.IListener
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

        private List<Component.EditAnimal> _editAnimalList = new();
        private List<Component.EditObject> _editObjectList = new();
        public Type.ETab CurrETabType { get; private set; } = Type.ETab.Animal;

        private bool _editing = false;

        public override void Initialize(Data data)
        {
            base.Initialize(data);            
        }

        public override void Activate()
        {
            base.Activate();
            
            ActiveContents();

            SetAnimalList();
            SetObjectList();

            _editing = false;
        }

        public EditList Setup(Type.ETab eTabType)
        {
            SetTab(eTabType);

            animalToggle?.SetIsOnWithoutNotify(eTabType == Type.ETab.Animal);
            objectToggle?.SetIsOnWithoutNotify(eTabType == Type.ETab.Object);

            return this;
        }

        private void SetAnimalList()
        {
            DeactviateAllAnimal();

            _editAnimalList.Clear();

            var infoList = MainGameManager.Get<AnimalManager>()?.AnimalInfoList;
            if(infoList == null)
                return;

            int placeId = GameUtils.ActivityPlaceId;

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
                };

                CreateEditAnimal(data);
            }
        }
        
        private void SetObjectList()
        {
            DeactviateAllObject();

            _editObjectList.Clear();

            var objectMgr = MainGameManager.Get<ObjectManager>();
            if (objectMgr == null)
                return;

            var infoList = objectMgr.ObjectInfoList;
            if (infoList == null)
                return;

            int placeId = GameUtils.ActivityPlaceId;

            foreach (var objectInfo in infoList)
            {
                if(objectInfo == null)
                    continue;

                var objectData = ObjectContainer.Instance.GetData(objectInfo.Id);
                if (objectData == null)
                    continue;

                if (objectData.PlaceId != placeId)
                    continue;

                int reaminCount = objectMgr.GetRemainCount(objectInfo.Id);
                if (reaminCount <= 0)
                    continue;

                var data = new Component.EditObject.Data()
                {
                    iListener = this,
                    ObjectId = objectInfo.Id,
                    Count = objectData.Count,
                    RemainCount = reaminCount,
                };

                CreateEditObject(data);
            }
        }

        private void CreateEditAnimal(Component.EditAnimal.Data data)
        {
            if (data == null)
                return;

            var editAnimal = new GameSystem.UICreator<UI.Component.EditAnimal, UI.Component.EditAnimal.Data>()
                      .SetData(data)
                      .SetRootRectTm(animalScrollRect.content)
                      .Create();

            _editAnimalList.Add(editAnimal);
        }

        private void CreateEditObject(Component.EditObject.Data objectData)
        {
            if (objectData == null)
                return;

            var editObject = new GameSystem.UICreator<UI.Component.EditObject, UI.Component.EditObject.Data>()
                   .SetData(objectData)
                   .SetRootRectTm(objectScrollRect.content)
                   .Create();

            _editObjectList.Add(editObject);
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
                    //SkinId = animalMgr.GetCurrenctSkinId(info.Id),
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

        public void RefreshObjectList(ObjectManager objectMgr)
        {
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

        #region EditAnimal.IListener
        void EditAnimal.IListener.Select(int id)
        {
            if (_editing)
                return;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            mainGameMgr.AddAnimalToPlace(id);

            _editing = true;

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

            mainGameMgr.AddObjectToPlace(id);

            _editing = true;
        }
        #endregion

        public void OnChanged(string tabType)
        {
            if(System.Enum.TryParse(tabType, out Type.ETab eTabType))
            {
                if(CurrETabType == eTabType)
                    return;
                
                SetTab(eTabType);
            }
        }

        public void OnClickClose()
        {
            _data?.IListener?.Close();
        }
    }
}

