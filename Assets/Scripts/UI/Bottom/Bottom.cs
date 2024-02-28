using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using DG.Tweening;
using Cysharp.Threading.Tasks;

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

            AllDeactivateGuideLine();
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

        public void SetInteractable(bool interactable, Game.Type.EBottomType[] exceptBottomTypes)
        {
            if (_bottomMenuList == null)
                return;

            foreach(var bottomMenu in _bottomMenuList)
            {
                if (bottomMenu == null)
                    continue;

                if(exceptBottomTypes != null)
                {
                    if (exceptBottomTypes.Contains(bottomMenu.EType))
                    {
                        bottomMenu.SetInteractable(!interactable);

                        continue;
                    }
                }

                bottomMenu.SetInteractable(interactable);
            }
        }

        public void AllDeactivateGuideLine()
        {
            if (_bottomMenuList == null)
                return;

            foreach (var bottomMenu in _bottomMenuList)
            {
                if (bottomMenu == null)
                    continue;

                bottomMenu.DeactivateGuideLine();
            }
        }

        public void ActivateGuideLine(Game.Type.EBottomType[] eBottomTypes)
        {
            AllDeactivateGuideLine();

            if (_bottomMenuList == null)
                return;

            foreach (var bottomMenu in _bottomMenuList)
            {
                if (bottomMenu == null)
                    continue;

                if (eBottomTypes != null)
                {
                    if (eBottomTypes.Contains(bottomMenu.EType))
                    {
                        bottomMenu.ActivateGuideLine();
                    }
                }
            }
        }

        #region EditList
        public void ActivateEditList(Game.Type.ETab eTabType, int index = -1)
        {
            ActivateEditListAsync(eTabType, index).Forget();
        }

        private async UniTask ActivateEditListAsync(Game.Type.ETab eTabType, int index = -1)
        {
            bool initialiae = false;
            if (EditList == null)
            {
                initialiae = true;

                EditList = new UICreator<EditList, EditList.Data>()
                    .SetData(new EditList.Data()
                    {
                        IListener = this,
                    })
                    .SetRootRectTm(EditListRootRectTm)
                    .Create();
            }

            EditList?.Setup(eTabType, index)?.Activate();

            if(initialiae)
            {
                await UniTask.Yield();
            }

            //EditList?.Setup(eTabType, index);
            //await UniTask.WaitForSeconds(0.5f);

            ActivateAnim(EditListRootRectTm, null);
        }

        public void ActivateEditListAfterDeactivateBottom(Game.Type.ETab eTabType, int index)
        {
            MainGameManager.Instance?.SetGameStateAsync(Game.Type.EGameState.Edit);

            DeactivateAnim(rootRectTm,
                () =>
                {
                    ActivateEditList(eTabType, index);
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
                            //.SetReInitialize(reInitialize)
                            .SetAnimActivateInterval(0.05f)
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
                            .SetAnimActivateInterval(0.05f)
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

            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);
        }
        #endregion

        #region Edit.IListener
        void EditList.IListener.Close()
        {
            MainGameManager.Instance?.SetGameStateAsync(Game.Type.EGameState.Game);

            DeactivateAnim(EditListRootRectTm,
                () =>
                {
                    ActivateAnim(rootRectTm, null);
                    EditList.Deactivate();

                    MainGameManager.Instance?.GameState?.End();
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