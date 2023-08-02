using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Game.Creature
{
    public class Animal : Base<Animal.Data>, UI.Element.IListener
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public int Order = 0;
            public Vector3 Pos = Vector3.zero;
            public bool IsGame = true;
        }

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;
        [SerializeField]
        private Animator animator = null;

        private AnimalRoot _animalRoot = null;
        private AnimalActionController _actionCtr = null;

        public BaseState<Creature.Animal> State { get; private set; } = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _animalRoot = GetComponentInChildren<AnimalRoot>();

            if (data != null)
            {
                transform.localPosition = data.Pos;

                SetSortingOrder(-(int)transform.localPosition.y);
            }

            InitActionController();

            element?.Init(this);
            ActiveEdit(false);
        }

        private void InitActionController()
        {
            _actionCtr = gameObject.GetOrAddComponent<AnimalActionController>();
            _actionCtr?.Init(animator, spriteRenderer, _data.IsGame);
        }

        public override void ChainUpdate()
        {
            _actionCtr?.ChainUpdate();
        }

        public override void OnTouchBegan(Camera gameCamera, GameSystem.Grid grid)
        {
            base.OnTouchBegan(gameCamera, grid);

            SetState(new Edit<Creature.Animal>(gameCamera, grid));

            SetSortingOrder(_selectOrder);
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            State?.Touch(touch);
        }
        
        public void SetState(BaseState<Creature.Animal> state)
        {
            if (state == null)
                return;

            if (state is Edit<Creature.Animal>)
            {
                EState_ = EState.Edit;
            }

            state.Apply(this);

            State = state;
        }

        private void SetSortingOrder(int order)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.sortingOrder = order;
        }

        #region Edit.IListener
        void UI.Element.IListener.Remove()
        {
            EState_ = EState.Remove;

            Command.Remove.Execute(Type.EElement.Animal, _data.Id);

            ActiveEdit(false);
        }

        void UI.Element.IListener.Arrange()
        {
            if (_data == null)
                return;

            EState_ = EState.Arrange;

            Command.Arrange.Execute(Type.EElement.Animal, _data.Id, transform.localPosition);

            SetSortingOrder(-(int)transform.localPosition.y);

            ActiveEdit(false);
        }
        #endregion
    }
}

