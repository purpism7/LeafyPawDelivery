using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game;
using UnityEngine.UI;

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
        }

        [Header("Skin")]
        [SerializeField]
        private int skinId = 0;
        [SerializeField]
        private Animator animator = null;

        private AnimalRoot _animalRoot = null;
        private AnimalActionController _actionCtr = null;

        public int SkinId { get { return skinId; } }
        public Game.Element.State.BaseState<Creature.Animal> State { get; private set; } = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            ElementData = AnimalContainer.Instance?.GetData(Id);

            _animalRoot = GetComponentInChildren<AnimalRoot>();

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

            _actionCtr?.Deactivate();
            DeactivateSpeechBubble();
        }

        private void InitActionController()
        {
            _actionCtr = gameObject.GetOrAddComponent<AnimalActionController>();
            _actionCtr?.Initialize(animator, spriteRenderer, !_data.IsEdit);
        }

        public override void ChainUpdate()
        {
            if (!IsActivate)
                return;

            _actionCtr?.ChainUpdate();
        }

        public override void OnTouchBegan(Touch? touch, GameSystem.GameCameraController gameCameraCtr, GameSystem.IGridProvider iGridProvider)
        {
            base.OnTouchBegan(touch, gameCameraCtr, iGridProvider);

            Game.State.Base gameState = MainGameManager.Instance?.GameState;
            if (gameState == null)
                return;

            if (gameState.CheckState<Game.State.Edit>())
            {
                SetState(new Game.Element.State.Edit<Creature.Animal>()?.Create(gameCameraCtr, iGridProvider));

                SetSortingOrder(_selectOrder);
                ActiveEdit(true);
            }
            else
            {
                SetState(new Game.Element.State.Game<Creature.Animal>()?.Create(gameCameraCtr, iGridProvider));
            }
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            State?.Touch(touch);
        }

        private void SetState(Game.Element.State.BaseState<Creature.Animal> state)
        {
            if (State != null &&
                state != null)
            {
                if (State.CheckEqual(state))
                    return;
            }

            if (state is Game.Element.State.Edit<Creature.Animal>)
            {
                StartEdit();
            }

            state?.Apply(this);

            State = state;
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
            EState_ = EState.Remove;

            Command.Remove.Execute(this);

            ActiveEdit(false);
            SetState(null);
        }

        void UI.Edit.IListener.Arrange()
        {
            if (_data == null)
                return;

            EState_ = EState.Arrange;

            Command.Arrange.Execute(this, transform.localPosition);

            SetSortingOrder(-(int)transform.localPosition.y);

            ActiveEdit(false);
            SetState(null);
        }
        #endregion
    }
}

