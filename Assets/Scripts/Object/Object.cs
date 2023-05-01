using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Game
{
    public class Object : Game.Base<Object.Data>, EditObject.IListener
    {
        public class Data : BaseData
        {
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

        #region Collision 
        private void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("OnCollisionEnter = " + collision.gameObject.name);
        }

        private void OnCollisionExit(Collision collision)
        {
            //Debug.Log("OnCollisionExit = " + collision.gameObject.name);
        }

        private void OnCollisionStay(Collision collision)
        {
            //Debug.Log("OnCollisionStay = " + collision.gameObject.name);

            var cell = collision.gameObject.GetComponent<GameSystem.Cell>();
            if(cell != null)
            {
                //Debug.Log(cell.Data_?.Column);
            }
        }
        #endregion

        #region EditObject.IListener
        void EditObject.IListener.Remove()
        {
            EState_ = EState.Remove;

            var cmd = new Command.Remove(ObjectUId);
            cmd?.Execute();

            ActiveEditObject(false);
        }

        void EditObject.IListener.Arrange()
        {
            EState_ = EState.Arrange;

            var cmd = new Command.Arrange(ObjectUId, transform.localPosition);
            cmd?.Execute();

            ActiveEditObject(false);
        }
        #endregion
    }
}

