using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Game;
using GameSystem;

namespace Game.Creature
{
    public interface IAnimal
    {
        void Remove(bool refresh);

        Animator Animator { get; }
        SpriteRenderer SpriteRenderer { get; }
        Game.Type.EGameState EGameState { get; }

        void Touch();
    }

    [ExecuteInEditMode]
    public class Animal : BaseElement<Animal.Data>, IAnimal, State.Conversation.IListener
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
        private int _interactionId = 0;

        public int SkinId { get { return skinId; } }

        private IPlaceState.EType _iPlaceState = IPlaceState.EType.None;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            var animalData = AnimalContainer.Instance?.GetData(Id);
            if (animalData != null)
            {
                _interactionId = animalData.InteractionId;
                ElementData = animalData;
            }
            
            _animalRoot = GetComponentInChildren<AnimalRoot>();

            if(_collider2D != null)
            {
                _animalRoot?.Initialize(_collider2D.bounds.center.y);
            }

            if (data != null)
            {
                if(data.IsEdit)
                {
                    CreateEdit(_animalRoot?.EditRootRectTm);
                }

                SetLocalPos(data.Pos);
                SetSortingOrder(-(int)LocalPos.y);
            }

            InitActionController();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            DeactivateChild().Forget();
        }

        private async UniTask DeactivateChild()
        {
            await UniTask.Yield();

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
                var editState = mainGameMgr.GameState?.Get<Game.State.Edit>();
                if (editState != null &&
                    editState.CheckIsEditElement(this))
                    return;

                SetState(new Element.State.Edit().Initialize(gameCameraCtr, iGrid));
                SetSortingOrder(SelectOrder);
                edit?.ActivateBottom();

                State?.Touch(TouchPhase.Began, null);

                return;
            }
           
            SetState(new Element.State.Play().Initialize(gameCameraCtr, iGrid));

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

        protected override void Return()
        {
            if (_spwaned)
                return;

            SetLocalPos(_data.Pos);

            Arrange();
        }

        protected override void Arrange()
        {
            if (_data == null)
                return;

            SetLocalPosZ(GameUtils.CalcPosZ(LocalPos.y));

            base.Arrange();
        }

        protected override void Conversation()
        {
            MainGameManager.Get<AnimalManager>()?.SetConverationAnimal(this);
            
            MainGameManager.Instance?.SetGameStateAsync(Type.EGameState.Conversation).Forget();
        }

        protected override void Special()
        {
            
        }

        public void StartSignatureAction()
        {
            _actionCtr?.StartSignatureAction();
        }

        #region IAnimal
        void IAnimal.Remove(bool refresh)
        {
            Remove(refresh);
        }

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

        void IAnimal.Touch()
        {
            DeactivateSpeechBubble();
            
            // SetState(new Element.State.Deactive().Initialize());
            // DeactivateChild().Forget();
            _actionCtr?.Deactivate();
            
            bool isInteracition = MainGameManager.Get<ObjectManager>().CheckExist(_interactionId);
            
            edit?.ActivateTopAsync(isInteracition,
                () =>
                {
                    
                }).Forget();
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
        
        #region State.Conversation.IListener

        void State.Conversation.IListener.Start()
        {
            // SetSortingOrder(SelectOrder);
        }
        
        void State.Conversation.IListener.Finish()
        {
            // SetSortingOrder(-(int)LocalPos.y);
            // MainGameManager.Get<AnimalManager>().AddFriendshipPoint(_data.Id, item.Value);
            _animalRoot?.AddFriendshipPoint(Id, 1, edit?.FriendshipPointRootRectTm);
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
                        SetState(new Element.State.Deactive().Initialize());

                        DeactivateChild().Forget();

                        break;
                    }
                case IPlaceState.EType.Edit:
                    {
                        _data.Pos = LocalPos;

                        DeactivateChild().Forget();

                        break;
                    }
            }
        }
    }
}

