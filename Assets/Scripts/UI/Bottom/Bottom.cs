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

        private const float InitPosY = -400f;

        #region Inspector        
        [SerializeField]
        private RectTransform EditListRootRectTm;
        #endregion
        
        private List<BottomMenu> BottomMenuList = new();

        public EditList EditList { get; private set; } = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            DeactivateAnim(EditListRootRectTm, null);
            InitBottomMenu();
        }

        private void InitBottomMenu()
        {
            if(!rootRectTm)
                return;

            var bottomMenus = rootRectTm.GetComponentsInChildren<BottomMenu>();
            foreach (var bottomMenu in bottomMenus)
            {
                bottomMenu?.Initialize(new BottomMenu.Data()
                {
                    ILisener = this,
                });
            }
        }

        public void ActivateAnim(System.Action completeAction)
        {
            ActivateAnim(rootRectTm, completeAction);
        }

        public void DeactivateAnim(System.Action endAction)
        {
            DeactivateAnim(rootRectTm, endAction);
        }

        #region EditList
        public void ActivateEditList(Game.Type.ETab eTabType)
        {
            if(EditList == null)
            {
                EditList = new UICreator<EditList, EditList.Data>()
                    .SetData(new EditList.Data()
                    {
                        IListener = this,
                    })
                    .SetRootRectTm(EditListRootRectTm)
                    .Create();
            }

            EditList.Setup(eTabType).Activate();
            ActivateAnim(EditListRootRectTm, null);
                    
            MainGameManager.Instance.SetGameState<Game.State.Edit>();
        }

        //public void ActivateEditList()
        //{
        //    EditList.Activate();
        //    ActivateAnim(EditListRootRectTm, null);
            
        //    MainGameManager.Instance.SetGameState<Game.State.Edit>();
        //}

        public void ActivateEditListAfterDeactivateBottom(Game.Type.ETab eTabType)
        {
            DeactivateAnim(rootRectTm,
                () =>
                {
                    ActivateEditList(eTabType);
                });
        }

        public void DeactivateEditList()
        {
            DeactivateAnim(EditListRootRectTm,
                () =>
                {
                    EditList.Deactivate();
                });
        }
        #endregion

        #region BottomMenu.IListener
        void BottomMenu.IListener.ClickBottomMenu()
        {
           
        }
        #endregion

        #region Edit.IListener
        void EditList.IListener.Close()
        {
            DeactivateAnim(EditListRootRectTm,
                () =>
                {
                    ActivateAnim(rootRectTm, null);
                    EditList.Deactivate();

                    MainGameManager.Instance.SetGameState<Game.State.Game>();
                });
        }
        #endregion

        private void ActivateAnim(RectTransform rectTm, System.Action completeAction)
        {
            if(!rectTm)
                return;

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(rectTm.DOAnchorPosY(0, 0.3f).SetEase(Ease.OutBack))
                .OnComplete(() =>
                {
                    completeAction?.Invoke();
                });
            sequence.Restart();
        }

        private void DeactivateAnim(RectTransform rectTm, System.Action completeAction)
        {
            if(!rectTm)
                return;

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