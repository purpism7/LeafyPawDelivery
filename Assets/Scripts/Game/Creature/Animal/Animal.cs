using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game;

namespace Game.Creature
{
    [ExecuteInEditMode]
    public class Animal : BaseElement<Animal.Data>, UI.Edit.IListener
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
                SetPos(data.Pos);
                SetSortingOrder(-(int)transform.localPosition.y);
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
            _actionCtr?.Initialize(Id, animator, spriteRenderer, !_data.IsEdit);
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

            Game.State.Base gameState = MainGameManager.Instance?.GameState;
            if (gameState == null)
                return;

            if (gameState.CheckState<Game.State.Edit>())
            {
                var editState = gameState.Get<Game.State.Edit>();
                if (editState != null &&
                    editState.CheckIsEditElement(this))
                    return;

                SetState(new Element.State.Edit()?.Initialize(gameCameraCtr, iGrid));

                SetSortingOrder(SelectOrder);
                ActiveEdit(true);
            }
            else
            {
                SetState(new Element.State.Play()?.Initialize(gameCameraCtr, iGrid));
            }

            if (touch != null)
            {
                State?.Touch(touch.Value);
            }
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            State?.Touch(touch);
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

        private void SetSortingOrder(int order)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.sortingOrder = order;
        }

        public void SetPos(Vector3 pos)
        {
            transform.localPosition = pos;
        }

        public void StartSignatureAction()
        {
            _actionCtr?.StartSignatureAction();
        }

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
        void UI.Edit.IListener.Remove()
        {
            Command.Remove.Execute(this);

            ActiveEdit(false);
            SetState(null);
        }

        void UI.Edit.IListener.Arrange()
        {
            if (_data == null)
                return;

            Command.Arrange.Execute(this, transform.localPosition);

            SetSortingOrder(-(int)transform.localPosition.y);

            ActiveEdit(false);
            SetState(null);
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

