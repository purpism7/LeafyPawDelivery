using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Map : BasePopup<Map.Data>
    {
        public class Data : BaseData
        {

        }
        
        [SerializeField] private ScrollRect placeScrollRect = null;
        
        private Game.Type.ETab _currETabType = Game.Type.ETab.Animal;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            InternalInitialize();
        }
        
        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));

            InternalInitialize();
            
            yield break;
        }

        private void InternalInitialize()
        {
            SetPlaceList();
        }

        private void SetPlaceList()
        {
            var datas = PlaceContainer.Instance.Datas;
            if (datas == null)
                return;

            foreach (var data in datas)
            {
                // var cell = new ComponentCreator<BookCell, BookCell.Data>()
                //     .SetData(new BookCell.Data()
                //     {
                //         IListener = this,
                //         Name = data.Name,
                //         IconSprite = GameSystem.ResourceManager.Instance.AtalsLoader.GetAnimalIconSprite(data.IconImgName),
                //     })
                //     .SetRootRectTm(animalScrollRect?.content)
                //     .Create();
            }
        }

        // private void SetObjectList()
        // {
        //     var datas = ObjectContainer.Instance?.Datas;
        //     if (datas == null)
        //         return;
        //
        //     foreach (var data in datas)
        //     {
        //         var cell = new ComponentCreator<BookCell, BookCell.Data>()
        //             .SetData(new BookCell.Data()
        //             {
        //                 IListener = this,
        //                 Name = data.Name,
        //                 IconSprite = GameSystem.ResourceManager.Instance.AtalsLoader.GetObjectIconSprite(data.IconImgName),
        //             })
        //             .SetRootRectTm(objectScrollRect?.content)
        //             .Create();
        //     }
        // }
    
        public override void Deactivate()
        {
            base.Deactivate();
        }
        
        // public void OnChanged(string tabType)
        // {
        //     if(System.Enum.TryParse(tabType, out Type.ETab eTabType))
        //     {
        //         if(_currETabType == eTabType)
        //         {
        //             return;
        //         }
        //
        //         _currETabType = eTabType;
        //
        //         ActiveContents();
        //     }
        // }
    }
}