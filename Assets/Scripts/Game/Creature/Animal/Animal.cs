using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Game;
using Game.Element.State;

namespace Game.Creature
{
    public interface IAnimal
    {
        void Remove(bool refresh);

        Animator Animator { get; }
        SpriteRenderer SpriteRenderer { get; }
        Game.Type.EGameState EGameState { get; }

        void Touch();
        Vector3 LocalPos { get; }
    }

    [ExecuteInEditMode]
    public class Animal : BaseElement<Animal.Data>, IAnimal, State.Conversation.IListener, Game.Element.State.Interaction.IListener
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
        
        private AnimalRoot _animalRoot = null;
        private IPlaceState.EType _iPlaceState = IPlaceState.EType.None;
        
        public AnimalActionController ActionCtr { get; private set; } = null;
        
        public int SkinId { get { return skinId; } }
        public bool ReadyToInteraction { get; private set; } = false;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);

            var animalData = AnimalContainer.Instance?.GetData(Id);
            if (animalData != null)
                ElementData = animalData;
            
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

            ActionCtr?.Deactivate();
        }

        private void InitActionController()
        {
            ActionCtr = gameObject.GetOrAddComponent<AnimalActionController>();
            ActionCtr?.Initialize(Id, this, !_data.IsEdit);
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

            ActionCtr?.ChainUpdate();
        }

        public override void OnTouchBegan(Touch? touch, GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            if (State is Interaction)
                return;
            
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

                SetState<Element.State.Edit>(gameCameraCtr, iGrid);
                SetSortingOrder(SelectOrder);
                edit?.ActivateBottom();

                State?.Touch(TouchPhase.Began, null);

                return;
            }
           
            SetState<Play>(gameCameraCtr, iGrid);

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
            IPlace iPlace = MainGameManager.Get<PlaceManager>().ActivityPlace;
            if (iPlace == null)
                return;
            
            var animalData = AnimalContainer.Instance?.GetData(Id);
            if (animalData == null)
                return;

            var specialObject = iPlace.ObjectList?.Find(obj => obj.IsActivate && obj.Id == animalData.InteractionId);
            if (specialObject != null)
                SetState<Interaction>();
            else
                MainGameManager.Instance?.SpawnSpecialObjectToPlace(animalData.InteractionId);
        }

        public void StartSignatureAction()
        {
            ActionCtr?.StartSignatureAction();
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
            
            ActionCtr?.Deactivate();

            bool isInteracition = false;
            var animalData = AnimalContainer.Instance?.GetData(Id);
            if (animalData != null)
                isInteracition = MainGameManager.Get<ObjectManager>().CheckExist(animalData.InteractionId);
            
            if (isInteracition)
            {
                ReadyToInteraction = true;
                
                edit?.ActivateTopAsync(isInteracition,
                    () =>
                    {
                        ReadyToInteraction = false;
                    }).Forget();
            }
            else
                StartSignatureAction();
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
            var animalMgr = MainGameManager.Get<AnimalManager>();
            if (animalMgr != null &&
                !animalMgr.CheckMaxFriendshipPoint(Id))
            {
                int addFriendPoint = 1;

                _animalRoot?.AddFriendshipPoint(Id, addFriendPoint, edit?.FriendshipPointRootRectTm);
            }

            StartSignatureAction();
        }
        #endregion
        
        #region Interaction.IListener

        void Interaction.IListener.Finish()
        {
            Activate();
            SetState<Play>();
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
                        SetState<DeActive>();

                        DeactivateChild().Forget();

                        break;
                    }
                case IPlaceState.EType.Edit:
                    {
                        _data.Pos = LocalPos;

                        if (State is Interaction)
                        {
                            ReadyToInteraction = false;
                            SetState<DeActive>();
                        }
                        
                        
                        DeactivateChild().Forget();

                        break;
                    }
            }
        }
    }
}

