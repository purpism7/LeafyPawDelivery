using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        
        private List<BottomMenu> _bottomMenuList = new();
        private Dictionary<Game.Type.EBottomType, bool> _reInitializeDic = new(); 
        private int _placeId = 0;

        public EditList EditList { get; private set; } = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            DeactivateAnim(EditListRootRectTm, null);
            InitializeBottomMenu();
        }

        private void InitializeBottomMenu()
        {
            _bottomMenuList?.Clear();
            _reInitializeDic?.Clear();

            if (!rootRectTm)
                return;

            var bottomMenus = rootRectTm.GetComponentsInChildren<BottomMenu>();
            foreach (var bottomMenu in bottomMenus)
            {
                if (bottomMenu == null)
                    continue;

                bottomMenu.Initialize(new BottomMenu.Data()
                {
                    ILisener = this,
                });

                _bottomMenuList?.Add(bottomMenu);
                _reInitializeDic?.Add(bottomMenu.EType, false);
            }
        }

        private void SetReInitialize()
        {
            bool reInitialize = false;
            int activityPlaceId = GameUtils.ActivityPlaceId;

            reInitialize = _placeId != activityPlaceId;
            if (reInitialize)
            {
                _reInitializeDic.Keys?.ToList().ForEach(
                    key =>
                    {
                        _reInitializeDic[key] = true;
                    });
            }

            _placeId = activityPlaceId;
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
        }

        public void ActivateEditListAfterDeactivateBottom(Game.Type.ETab eTabType)
        {
            MainGameManager.Instance?.SetGameState<Game.State.Edit>();

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
        void BottomMenu.IListener.SelectBottomMenu(Game.Type.EBottomType eBottomType)
        {
            SetReInitialize();

            bool reInitialize = false;
            if(_reInitializeDic != null &&
               _reInitializeDic.TryGetValue(eBottomType, out reInitialize))
            {
                _reInitializeDic[eBottomType] = false;
            }
            
            switch (eBottomType)
            {
                case Game.Type.EBottomType.Shop:
                    {
                        var popup = new GameSystem.PopupCreator<Shop, Shop.Data_>()
                            .SetCoInit(true)
                            .SetReInitialize(reInitialize)
                            .Create();

                        break;
                    }

                case Game.Type.EBottomType.Arrangement:
                    {
                        var popup = new GameSystem.PopupCreator<Arrangement, Arrangement.Data>()
                            .SetCoInit(true)
                            .SetReInitialize(reInitialize)
                            .SetData(new Arrangement.Data()
                            {
                                PlaceId = _placeId,
                            })
                            .Create();

                        break;
                    }

                case Game.Type.EBottomType.Book:
                    {
                        var popup = new GameSystem.PopupCreator<Book, Book.Data>()
                             .SetCoInit(true)
                             .SetReInitialize(reInitialize)
                             .SetData(new Book.Data()
                             {
                                 PlaceId = _placeId,
                             })
                             .Create();

                        break;
                    }


                case Game.Type.EBottomType.Acquire:
                    {
                        var popup = new GameSystem.PopupCreator<Acquire, Acquire.Data>()
                                .SetCoInit(true)
                                .Create();

                        break;
                    }

                case Game.Type.EBottomType.Map:
                    {
                        var popup = new GameSystem.PopupCreator<Map, Map.Data>()
                            .SetCoInit(true)
                            .Create();

                        break;
                    }
            }
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