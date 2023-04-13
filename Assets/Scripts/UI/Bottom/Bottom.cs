using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using GameSystem;

namespace UI
{
    public class Bottom : Common<Bottom.Data>, BottomMenu.IListener, Edit.IListener
    {
        public class Data : BaseData
        {
            public RectTransform PopupRootRectTm = null;
        }

        readonly float InitPosY = -500f;

        public RectTransform RootRectTm;
        [SerializeField]
        private RectTransform EditRootRectTm;

        private List<BottomMenu> BottomMenuList = new();
        private Edit _edit = null;

        public override void Init(Data data)
        {
            base.Init(data);

            HideAnim(EditRootRectTm, null);
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

        private void ShowEdit()
        {
            if(_edit == null)
            {
                _edit = new UICreator<Edit, Edit.Data>()
                    .SetData(new Edit.Data()
                    {
                        IListener = this,
                    })
                    .SetRootRectTm(EditRootRectTm)
                    .Create();
            }

            UIUtils.SetActive(EditRootRectTm, true);
            ShowAnim(EditRootRectTm, null);

            GameSystem.GameManager.Instance.SetGameState<Game.State.Edit>();
        }

        #region BottomMenu.IListener
        void BottomMenu.IListener.ClickBottomMenu()
        {
            //var popup = new GameSystem.PopupCreator<Arrangement, Arrangement.Data>()
            //    .SetData(new Arrangement.Data()
            //    {

            //    })
            //    .Create();

            HideAnim(RootRectTm,
                () =>
                {
                    ShowEdit();
                });
        }
        #endregion

        #region Edit.IListener
        void Edit.IListener.Close()
        {
            HideAnim(EditRootRectTm,
                () =>
                {
                    ShowAnim(RootRectTm, null);
                    UIUtils.SetActive(EditRootRectTm, false);

                    GameSystem.GameManager.Instance.SetGameState<Game.State.Game>();
                });
        }
        #endregion

        private void ShowAnim(RectTransform rectTm, System.Action completeAction)
        {
            if(!rectTm)
            {
                return;
            }

            Sequence sequence = DOTween.Sequence()
             .SetAutoKill(false)
             .Append(rectTm.DOAnchorPosY(0, 0.3f).SetEase(Ease.OutBack))

             .OnComplete(() =>
             {
                 completeAction?.Invoke();
             });
            sequence.Restart();
        }

        private void HideAnim(RectTransform rectTm, System.Action completeAction)
        {
            if(!rectTm)
            {
                return;
            }

            Sequence sequence = DOTween.Sequence()
              .SetAutoKill(false)
              .Append(rectTm.DOAnchorPosY(InitPosY, 0.3f).SetEase(Ease.InBack))

              .OnComplete(() =>
              {
                  completeAction?.Invoke();
              });
            sequence.Restart();
        }
    }
}

