using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class Bottom : Common<Bottom.Data>, BottomMenu.IListener
    {
        public class Data : UI.Data
        {
            public RectTransform PopupRootRectTm = null;
        }

        public RectTransform RootRectTm;
        [SerializeField]
        private RectTransform EditRootRectTm;

        private List<BottomMenu> BottomMenuList = new();

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
                    ShowAnim(EditRootRectTm, null);
                });
        }

        private void ShowAnim(RectTransform rectTm, System.Action completeAction)
        {
            if(!rectTm)
            {
                return;
            }

            Sequence sequence = DOTween.Sequence()
             .SetAutoKill(false)
             .Append(rectTm.DOAnchorPosY(500f, 0.3f).SetEase(Ease.OutBack))

             .OnComplete(() =>
             {
                 completeAction?.Invoke();
             });
            sequence.Restart();

            //Sequence sequence = DOTween.Sequence()
            //   .SetAutoKill(false)
            //   .AppendCallback(() =>
            //   {
            //       Vector2.MoveTowards(rectTm.anchoredPosition, new Vector2(0, -500f), Time.deltaTime * 5f); 
            //   })
            //   .AppendInterval(1f)
            //   //.Append(tm.DOMoveY(1000f, 0.5f).SetEase(Ease.OutBack))
            //   .OnComplete(() =>
            //   {
            //       completeAction?.Invoke();
            //   });
            //sequence.Restart();
        }

        private void HideAnim(RectTransform rectTm, System.Action completeAction)
        {
            if(!rectTm)
            {
                return;
            }

            //Sequence sequence = DOTween.Sequence()
            //    .SetAutoKill(false)
            //    .Append(rectTm.DOLocalMoveY(-500f, 0.5f).SetEase(Ease.OutBack))
            //    .OnComplete(() =>
            //    {
            //        completeAction?.Invoke();
            //    });
            //sequence.Restart();

            Sequence sequence = DOTween.Sequence()
              .SetAutoKill(false)
              .Append(rectTm.DOAnchorPosY(-500f, 0.3f).SetEase(Ease.InBack))

              .OnComplete(() =>
              {
                  completeAction?.Invoke();
              });
            sequence.Restart();
        }
    }
}

