using System.Collections;
using System.Collections.Generic;
using UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EditList : Base<EditList.Data>
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

        private List<Component.EditObject> _editObjectList = new();
        public Type.ETab CurrETabType { get; private set; } = Type.ETab.Animal;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetAnimalList();
            SetObjectList();
        }

        public override void Activate()
        {
            base.Activate();
            
            ActiveContents();
        }

        public EditList Setup(Type.ETab eTabType)
        {
            SetTab(eTabType);

            if (animalToggle != null)
            {
                animalToggle.isOn = eTabType == Type.ETab.Animal; 
            }

            if (objectToggle != null)
            {
                objectToggle.isOn = eTabType == Type.ETab.Object;
            }
            
            return this;
        }

        private void SetAnimalList()
        {
            // _editObjectList.Clear();

            var infoList = MainGameManager.Instance?.AnimalMgr?.AnimalInfoList;
            if(infoList == null)
            {
                return;
            }

            foreach(var info in infoList)
            {
                if(info == null)
                    continue;

                if(info.PlaceId > 0)
                    continue;

                var data = new Component.EditAnimal.Data()
                {
                    AnimalData = AnimalContainer.Instance.GetData(info.Id),
                };

                var editObject = new GameSystem.UICreator<UI.Component.EditAnimal, UI.Component.EditAnimal.Data>()
                    .SetData(data)
                    .SetRootRectTm(animalScrollRect.content)
                    .Create();
            }
        }
        
        private void SetObjectList()
        {
            _editObjectList.Clear();

            var objectInfoList = MainGameManager.Instance?.ObjectMgr?.ObjectInfoList;
            if(objectInfoList == null)
                return;

            foreach(var objectInfo in objectInfoList)
            {
                if(objectInfo == null)
                    continue;

                if(objectInfo.PlaceId > 0)
                    continue;

                var data = new Component.EditObject.Data()
                {
                    ObjectId = objectInfo.Id,
                    ObjectUId = objectInfo.UId,
                };

                CreateEditObject(data);
            }
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

        private void DeactviateAllObject()
        {
            foreach(var objectInfo in _editObjectList)
            {
                objectInfo?.gameObject.SetActive(false);
            }
        }

        public void RefreshObjectList()
        {
            var objectInfoList = MainGameManager.Instance?.ObjectMgr?.ObjectInfoList;
            if (objectInfoList == null)
                return;

            DeactviateAllObject();

            for (int i = 0; i < objectInfoList.Count; ++i)
            {
                var objectInfo = objectInfoList[i];
                if (objectInfo == null)
                    continue;

                if (objectInfo.PlaceId > 0)
                    continue;

                var data = new Component.EditObject.Data()
                {
                    ObjectId = objectInfo.Id,
                    ObjectUId = objectInfo.UId,
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

