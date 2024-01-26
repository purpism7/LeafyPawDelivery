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

        public Game.Type.EBottomType EType { get { return _eType; } }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            System.Enum.TryParse(gameObject.name, out _eType);

            btn?.onClick.RemoveAllListeners();
            btn?.onClick.AddListener(OnClick);

            RegisterNotification();

            UIUtils.SetActive(redDotRectTm, false);
        }

        public override void Activate()
        {
            base.Activate();

            //SetNotification();
        }

        private void RegisterNotification()
        {
            switch (_eType)
            {
                case Game.Type.EBottomType.Map:
                    {
                        Game.Notification.Get?.AddListener(Game.Notification.EType.OpenPlace, this);

                        break;
                    }

                case Game.Type.EBottomType.Book:
                    {
                        Game.Notification.Get?.AddListener(Game.Notification.EType.AddAnimal, this);
                        Game.Notification.Get?.AddListener(Game.Notification.EType.AddObject, this);

                        break;
                    }

                case Game.Type.EBottomType.Acquire:
                    {
                        Game.Notification.Get?.AddListener(Game.Notification.EType.CompleteDailyMission, this);
                        Game.Notification.Get?.AddListener(Game.Notification.EType.CompleteAchievement, this);

                        break;
                    }
            }
        }

        private void SetNotification()
        {
            var connector = Info.Connector.Get;
            if (connector == null)
                return;

            switch (_eType)
            {
                case Game.Type.EBottomType.Map:
                    {
                        UIUtils.SetActive(redDotRectTm, connector.OpenPlaceId > 0);

                        break;
                    }

                case Game.Type.EBottomType.Book:
                    {
                        UIUtils.SetActive(redDotRectTm, connector.AddAnimalId > 0 || connector.AddObjectId > 0);

                        break;
                    }

                case Game.Type.EBottomType.Acquire:
                    {
                        UIUtils.SetActive(redDotRectTm, connector.IsCompleteDailyMission || connector.IsCompleteAchievement);

                        break;
                    }
            }
        }

        #region Notification.IListener
        void Game.Notification.IListener.Notify()
        {
            SetNotification();
        }
        #endregion

        public void OnClick()
        {
            _data?.ILisener?.SelectBottomMenu(_eType);
        }
    }
}

