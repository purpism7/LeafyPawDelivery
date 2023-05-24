using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BottomMenu : Base<BottomMenu.Data>
    {
        public enum EType
        {
            None,

            Shop,
            Arrangement,
            Book,
            Map,
        }

        public interface IListener
        {
            void ClickBottomMenu();
        }

        public class Data : BaseData
        {
            public IListener ILisener = null;
        }

        [SerializeField] private Button btn = null;

        public override void Init(Data data)
        {
            base.Init(data);
            
            btn?.onClick.RemoveAllListeners();
            btn?.onClick.AddListener(OnClick);
        }

        public override void Activate()
        {
            base.Activate();
        }

        public void OnClick()
        {
            Debug.Log(gameObject.name);
            if (System.Enum.TryParse(gameObject.name, out EType eType))
            {
                switch(eType)
                {
                    case EType.Shop:
                        {
                            var popup = new GameSystem.PopupCreator<Shop, Shop.Data>()
                                .Create();
                        }
                        break;
                    
                    case EType.Arrangement:
                        {
                            var popup = new GameSystem.PopupCreator<Arrangement, Arrangement.Data>()
                                .SetCoInit(true)
                                .Create();
                        }
                        break;
                    
                    case EType.Book:
                        {
                            var popup = new GameSystem.PopupCreator<Book, Book.Data>()
                                .SetCoInit(true)
                                .Create();
                        }
                        break;
                    
                    case EType.Map:
                        {
                            var popup = new GameSystem.PopupCreator<Map, Map.Data>()
                                .SetCoInit(true)
                                .Create();
                        }
                        break;

                    default:
                        {
                            _data?.ILisener?.ClickBottomMenu();
                        }
                        break;
                }
            }
        }
    }
}

