using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Game.Creature
{
    public class Animal : Base<Animal.Data>, UI.Edit.IListener
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public int Order = 0;
            public Vector3 Pos = Vector3.zero;
            public System.Action<DropItem, Transform> DropItemAction = null;
        }

        private AnimalRoot _animalRoot = null;
        private AnimalActionController _actionCtr = null;
        private SpriteRenderer _spriteRenderer = null;

        public BaseState<Creature.Animal> State { get; private set; } = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _animalRoot = GetComponentInChildren<AnimalRoot>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (data != null)
            {
                transform.localPosition = data.Pos;

                SetSortingOrder(-(int)transform.localPosition.y);
            }

            InitActionController();
            edit?.Init(this);

            ActiveEdit(false);
        }

        private void InitActionController()
        {
            _actionCtr = gameObject.GetOrAddComponent<AnimalActionController>();
            _actionCtr?.Init();
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
            if (_spriteRenderer == null)
                return;

            _spriteRenderer.sortingOrder = order;
        }

        #region Edit.IListener
        void UI.Edit.IListener.Remove()
        {
            EState_ = EState.Remove;

            Command.Remove.Execute(Type.EMain.Animal, _data.Id);

            ActiveEdit(false);
        }

        void UI.Edit.IListener.Arrange()
        {
            if (_data == null)
                return;

            EState_ = EState.Arrange;

            Command.Arrange.Execute(Type.EMain.Animal, _data.Id, transform.localPosition);

            SetSortingOrder(-(int)transform.localPosition.y);

            ActiveEdit(false);
        }
        #endregion
    }
}

