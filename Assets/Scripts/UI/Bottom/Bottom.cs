using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using GameSystem;

namespace UI
{
    public class Bottom : Common<Bottom.Data>, BottomMenu.IListener, EditList.IListener
    {
        public class Data : BaseData
        {
            public RectTransform PopupRootRectTm = null;
        }

        readonly float InitPosY = -500f;

        public RectTransform RootRectTm;
        [SerializeField]
        private RectTransform EditListRootRectTm;

        private List<BottomMenu> BottomMenuList = new();
        private EditList _editList = null;

        public override void Init(Data data)
        {
            base.Init(data);

            HideAnim(EditListRootRectTm, null);
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

        private void ShowEditList()
        {
            if(_editList == null)
            {
                _editList = new UICreator<EditList, EditList.Data>()
                    .SetData(new EditList.Data()
                    {
                        IListener = this,
                    })
                    .SetRootRectTm(EditListRootRectTm)
                    .Create();
            }

            UIUtils.SetActive(EditListRootRectTm, true);
            ShowAnim(EditListRootRectTm, null);

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
                    ShowEditList();
                });
        }
        #endregion

        #region Edit.IListener
        void EditList.IListener.Close()
        {
            HideAnim(EditListRootRectTm,
                () =>
                {
                    ShowAnim(RootRectTm, null);
                    UIUtils.SetActive(EditListRootRectTm, false);

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

