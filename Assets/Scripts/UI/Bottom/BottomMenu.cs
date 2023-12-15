using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class BottomMenu : Base<BottomMenu.Data>, Game.Notification.IListener
    {
        public class Data : BaseData
        {
            public IListener ILisener = null;
        }

        public interface IListener
        {
            void SelectBottomMenu(Game.Type.EBottomType eType);
        }

        [SerializeField] private Button btn = null;
        [SerializeField]
        private RectTransform redDotRectTm = null;

        private Game.Type.EBottomType _eType = Game.Type.EBottomType.None;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            System.Enum.TryParse(gameObject.name, out _eType);

            btn?.onClick.RemoveAllListeners();
            btn?.onClick.AddListener(OnClick);

            SetNotification();

            UIUtils.SetActive(redDotRectTm, false);
        }

        public override void Activate()
        {
            base.Activate();
        }

        private void SetNotification()
        {
            switch (_eType)
            {
                case Game.Type.EBottomType.Map:
                    {
                        Game.Notification.Get?.AddListener(Game.Notification.EType.OpenPlace, this);

                        break;
                    }
            }
        }

        #region Notification.IListener
        void Game.Notification.IListener.Notify()
        {
            switch (_eType)
            {
                case Game.Type.EBottomType.Map:
                    {
                        var connector = new Info.Connector();
                        UIUtils.SetActive(redDotRectTm, connector.OpenPlaceId > 0);

                        break;
                    }
            }
        }
        #endregion

        public void OnClick()
        {
            _data?.ILisener?.SelectBottomMenu(_eType);
        }
    }
}

