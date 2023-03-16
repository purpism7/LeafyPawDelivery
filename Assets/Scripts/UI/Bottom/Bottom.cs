using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Bottom : Base<Bottom.Data>, BottomMenu.IListener
    {
        public class Data : UI.Data
        {
            public RectTransform PopupRootRectTm = null;
        }

        public RectTransform RootRectTm;

        private List<BottomMenu> BottomMenuList = new();

        public override void Init(Data data)
        {
            base.Init(data);

            InitBttomMenu();
        }

        private void InitBttomMenu()
        {
            if(!RootRectTm)
            {
                return;
            }

            var bottomMenus = RootRectTm.GetComponentsInChildren<BottomMenu>();
            foreach (var bottomMenu in bottomMenus)
            {
                bottomMenu?.Init(new BottomMenu.Data()
                {
                    ILisener = this,
                });
            }
        }

        void BottomMenu.IListener.ClickBottomMenu()
        {
            var popup = new GameSystem.PopupCreator<Arrangement, Arrangement.Data>()
                .SetData(new Arrangement.Data()
                {
                    
                })
                .Create();
        }
    }
}

