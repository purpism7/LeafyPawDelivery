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

        private List<Component.EditObject> _editObjectList = new();
        private Type.ETab _currETabType = Type.ETab.Animal;

        public override void Init(Data data)
        {
            base.Init(data);

            SetAnimalList();
            SetObjectList();

            ActiveContents();
        }

        private void SetAnimalList()
        {
            // _editObjectList.Clear();

            var infoList = GameSystem.GameManager.Instance?.AnimalMgr?.AnimalInfoList;
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

            var objectInfoList = GameSystem.GameManager.Instance?.ObjectMgr?.ObjectInfoList;
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
            UIUtils.SetActive(animalScrollRect?.gameObject, _currETabType == Type.ETab.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, _currETabType == Type.ETab.Object);
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
            var objectInfoList = GameSystem.GameManager.Instance?.ObjectMgr?.ObjectInfoList;
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
                        editObj.Init(data);
                        editObj.gameObject.SetActive(true);
                    }
                }
                else
                {
                    CreateEditObject(data);
                }
            }
        }
        
        public void OnChanged(string tabType)
        {
            if(System.Enum.TryParse(tabType, out Type.ETab eTabType))
            {
                if(_currETabType == eTabType)
                {
                    return;
                }

                _currETabType = eTabType;

                ActiveContents();
            }
        }

        public void OnClickClose()
        {
            _data?.IListener?.Close();
        }
    }
}

