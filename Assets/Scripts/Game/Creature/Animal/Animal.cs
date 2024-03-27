using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game;

namespace Game.Creature
{
    public interface IAnimal
    {
        Animator Animator { get; }
        SpriteRenderer SpriteRenderer { get; }
        Game.Type.EGameState EGameState { get; }
    }

    [ExecuteInEditMode]
    public class Animal : BaseElement<Animal.Data>, UI.Edit.IListener, IAnimal
    {
        public class Data : BaseData
        {
            public int Order = 0;
            public Vector3 Pos = Vector3.zero;
            public bool IsEdit = true;
            public bool IsSpeechBubble = true;
            public IPlaceState IPlaceState = null;
        }

        [Header("Skin")]
        [SerializeField]
        private int skinId = 0;
        [SerializeField]
        private Animator animator = null;

        private AnimalRoot _animalRoot = null;
        private AnimalActionController _actionCtr = null;

        public int SkinId { get { return skinId; } }

        private IPlaceState.EType _iPlaceState = IPlaceState.EType.None;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            ElementData = AnimalContainer.Instance?.GetData(Id);

            _animalRoot = GetComponentInChildren<AnimalRoot>();

            if(_collider2D != null)
            {
                _animalRoot?.Initialize(_collider2D.bounds.center.y);
            }

            CreateEdit();

            if (data != null)
            {
                SetLocalPos(data.Pos);
                SetSortingOrder(-(int)LocalPos.y);
            }

            InitActionController();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            DeactivateChild();
        }

        private void DeactivateChild()
        {
            _actionCtr?.Deactivate();
        }

        private void InitActionController()
        {
            _actionCtr = gameObject.GetOrAddComponent<AnimalActionController>();
            _actionCtr?.Initialize(Id, this, !_data.IsEdit);
        }

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            var iPlaceState = _data?.IPlaceState;
            if (iPlaceState != null)
            {
                SetPlaceState(iPlaceState.State);
            }
            
            if (_iPlaceState != IPlaceState.EType.Active)
                return;

            if (!IsActivate)
                return;

            _actionCtr?.ChainUpdate();
        }

        public override void OnTouchBegan(Touch? touch, GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            base.OnTouchBegan(touch, gameCameraCtr, iGrid);

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            var eGameState = mainGameMgr.EGameState;
    
            if (eGameState == Type.EGameState.Edit)
            {
                var editState = mainGameMgr?.GameState?.Get<Game.State.Edit>();
                if (editState != null &&
                    editState.CheckIsEditElement(this))
                    return;

                SetState(new Element.State.Edit()?.Initialize(gameCameraCtr, iGrid));

                if(!_spwaned)
                {
                    _data.Pos = LocalPos;
                }

                SetSortingOrder(SelectOrder);
                ActiveEdit(true);

                State?.Touch(TouchPhase.Began, null);

                return;
            }
            else
            {
                SetState(new Element.State.Play()?.Initialize(gameCameraCtr, iGrid));
            }

            if (touch != null)
            {
                State?.Touch(touch.Value.phase, touch.Value);
            }
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            State?.Touch(touch.phase, touch);
        }

        private void CreateEdit()
        {
            if (_data == null)
                return;

            if (!_data.IsEdit)
                return;

            edit = new GameSystem.UICreator<UI.Edit, UI.Edit.Data>()
                .SetData(new UI.Edit.Data()
                {
                    IListener = this,
                })
                .SetRootRectTm(_animalRoot?.EditRootRectTm)
                .Create();

            ActiveEdit(false);
        }

        public void StartSignatureAction()
        {
            _actionCtr?.StartSignatureAction();
        }

        #region IAnimal
        Animator IAnimal.Animator
        {
            get
            {
                return animator;
            }
        }

        SpriteRenderer IAnimal.SpriteRenderer
        {
            get
            {
                return spriteRenderer;
            }
        }

        Game.Type.EGameState IAnimal.EGameState
        {
            get
            {
                var mainGameMgr = MainGameManager.Instance;
                if (mainGameMgr == null)
                    return Game.Type.EGameState.None;

                return mainGameMgr.EGameState;
            }
        }
        #endregion

        #region SpeechBubble
        public void ActivateSpeechBubble(System.Action endAction)
        {
            _animalRoot?.ActivateSpeechBubble(endAction);
        }

        public void DeactivateSpeechBubble()
        {
            _animalRoot?.DeactivateSpeechBubble();
        }
        #endregion

        #region Edit.IListener
        void UI.Edit.IListener.Return()
        {
            SetLocalPos(_data.Pos);

            Arrange();
        }

        void UI.Edit.IListener.Remove()
        {
            Remove();
        }

        void UI.Edit.IListener.Arrange()
        {
            if (_data == null)
                return;

            SetLocalPosZ(GameUtils.CalcPosZ(LocalPos.y));

            Arrange();
        }
        #endregion

        private void SetPlaceState(IPlaceState.EType state)
        {
            if (_iPlaceState == state)
                return;

            _iPlaceState = state;
            
            switch(state)
            {
                case IPlaceState.EType.Deactive:
                    {
                        SetState(new Element.State.Deactive()?.Initialize());

                        DeactivateChild();

                        break;
                    }
                case IPlaceState.EType.Edit:
                    {
                        DeactivateChild();

                        break;
                    }
            }
        }
    }
}

