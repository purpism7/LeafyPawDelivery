using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI;

namespace Game
{
    public class Object : Game.Base<Object.Data>, EditObject.IListener
    {
        readonly private int SelectOrder = 1000;

        public class Data : BaseData
        {
            public int ObjectId = 0;
            public int ObjectUId = 0;
            public Vector3 Pos = Vector3.zero;
        }

        #region Inspector
        public UI.EditObject EditObject = null;
        public SpriteRenderer ObjectSprRenderer = null;
        #endregion

        public int ObjectUId { get { return _data != null ? _data.ObjectUId : 0; } }
        public ObjectState State { get; private set; } = null;

        public override void Init(Data data)
        {
            base.Init(data);

            if(data != null)
            {
                UId = ObjectUId;
                transform.localPosition = data.Pos;

                SetSortingOrder(-(int)transform.localPosition.y);
            }

            EditObject?.Init(this);
            ActiveEditObject(false);
        }

        public override void ChainUpdate()
        {
            return;
        }

        // object 최초 선택 시, 호출.
        public override void OnTouchBegan(Camera gameCamera, GameSystem.Grid grid)
        {
            base.OnTouchBegan(gameCamera, grid);

            SetState(new Edit(gameCamera, grid));

            SetSortingOrder(SelectOrder);
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            State?.Touch(touch);
        }

        public void SetState(ObjectState state)
        {
            if(state == null)
            {
                return;
            }

            if(state is Edit)
            {
                EState_ = EState.Edit;
            }

            state.Apply(this);

            State = state;
        }

        public void ActiveEditObject(bool active)
        {
            UIUtils.SetActive(EditObject?.CanvasRectTm, active);
        }

        private void SetSortingOrder(int order)
        {
            if (ObjectSprRenderer == null)
            {
                return;
            }

            ObjectSprRenderer.sortingOrder = order;
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

        #region EditObject.IListener
        void EditObject.IListener.Remove()
        {
            EState_ = EState.Remove;

            Command.Remove.Execute(_data.ObjectId, ObjectUId);

            ActiveEditObject(false);
        }

        void EditObject.IListener.Arrange()
        {
            EState_ = EState.Arrange;

            Command.Arrange.Execute(ObjectUId, transform.localPosition);

            SetSortingOrder(-(int)transform.localPosition.y);

            ActiveEditObject(false);
        }
        #endregion
    }
}

