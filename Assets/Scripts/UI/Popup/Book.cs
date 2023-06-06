using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Book : Base<Book.Data>, BookCell.IListener
    {
        public class Data : BaseData
        {

        }
        
        [SerializeField] private ScrollRect animalScrollRect = null;
        [SerializeField] private ScrollRect objectScrollRect = null;

        private Type.ETab _currETabType = Type.ETab.Animal;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            InternalInit();
        }
        
        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            InternalInit();

            Debug.Log("book init");

            yield return null;

            Debug.Log("End book init");
        }

        private void InternalInit()
        {
            SetAnimalList();
            SetObjectList();

            ActiveContents();
        }

        private void SetAnimalList()
        {
            var datas = AnimalContainer.Instance.Datas;
            if (datas == null)
                return;

            foreach (var data in datas)
            {
                var cell = new ComponentCreator<BookCell, BookCell.Data>()
                    .SetData(new BookCell.Data()
                    {
                        IListener = this,
                        Name = data.Name,
                    })
                    .SetRootRectTm(animalScrollRect?.content)
                    .Create();
            }
        }

        private void SetObjectList()
        {
            var datas = ObjectContainer.Instance?.Datas;
            if (datas == null)
                return;

            foreach (var data in datas)
            {
                var cell = new ComponentCreator<BookCell, BookCell.Data>()
                    .SetData(new BookCell.Data()
                    {
                        IListener = this,
                        Name = data.Name,
                    })
                    .SetRootRectTm(objectScrollRect?.content)
                    .Create();
            }
        }

        private void ActiveContents()
        {
            UIUtils.SetActive(animalScrollRect?.gameObject, _currETabType == Type.ETab.Animal);
            UIUtils.SetActive(objectScrollRect?.gameObject, _currETabType == Type.ETab.Object);
        }

        public override void Deactivate()
        {
            base.Deactivate();
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

        #region BookCell.IListener
        void BookCell.IListener.Click()
        {

        }
        #endregion
    }
}