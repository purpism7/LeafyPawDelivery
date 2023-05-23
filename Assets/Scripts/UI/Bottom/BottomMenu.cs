using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }

        public interface IListener
        {
            void ClickBottomMenu();
        }

        public class Data : BaseData
        {
            public IListener ILisener = null;
        }

        public override void Init(Data data)
        {
            base.Init(data);
        }

        public override void Activate()
        {
            base.Activate();
        }

        public void OnClick(string eTypeStr)
        {
            if (System.Enum.TryParse(eTypeStr, out EType eType))
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

