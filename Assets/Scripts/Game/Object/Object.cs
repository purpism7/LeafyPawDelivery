using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;

using UI;

namespace Game
{
    public class Object : Game.BaseElement<Object.Data>, UI.Edit.IListener
    {
        public class Data : BaseData
        {
            public int ObjectId = 0;
            public int ObjectUId = 0;
            public Vector3 Pos = Vector3.zero;
        }

        public int ObjectUId { get { return _data != null ? _data.ObjectUId : 0; } }
        public BaseState<Game.Object> State { get; private set; } = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            EElement = Type.EElement.Object;

            if(data != null)
            {
                UId = ObjectUId;
                transform.localPosition = data.Pos;

                SetSortingOrder(-(int)transform.localPosition.y);
            }

            edit?.Initialize(new Edit.Data()
            {
                IListener = this,
            });

            ActiveEdit(false);
        }

        public override void ChainUpdate()
        {
            return;
        }

        // object 최초 선택 시, 호출.
        public override void OnTouchBegan(Camera gameCamera, GameSystem.Grid grid)
        {
            base.OnTouchBegan(gameCamera, grid);

            var edit = new Game.Edit<Game.Object>(gameCamera, grid);

            SetState(edit);

            SetSortingOrder(_selectOrder);
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            State?.Touch(touch);
        }

        public void SetState(BaseState<Game.Object> state)
        {
            if(state == null)
                return;

            if (state is Game.Edit<Game.Object>)
            {
                EState_ = EState.Edit;

                SetOutline(5f);
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

        #region Collision 
        //private void OnCollisionEnter(Collision collision)
        //{
        //    //Debug.Log("OnCollisionEnter = " + collision.gameObject.name);
        //}

        //private void OnCollisionExit(Collision collision)
        //{
        //    //Debug.Log("OnCollisionExit = " + collision.gameObject.name);
        //}

        //private void OnCollisionStay(Collision collision)
        //{
        //    //Debug.Log("OnCollisionStay = " + collision.gameObject.name);

        //    var cell = collision.gameObject.GetComponent<GameSystem.Cell>();
        //    if(cell != null)
        //    {
        //        //Debug.Log(cell.Data_?.Column);
        //    }
        //}
        #endregion

        #region Edit.IListener
        void UI.Edit.IListener.Remove()
        {
            EState_ = EState.Remove;

            Command.Remove.Execute(this);

            ActiveEdit(false);
        }

        void UI.Edit.IListener.Arrange()
        {
            EState_ = EState.Arrange;

            Command.Arrange.Execute(this, transform.localPosition);

            SetSortingOrder(-(int)transform.localPosition.y);

            ActiveEdit(false);
        }
        #endregion
    }
}

