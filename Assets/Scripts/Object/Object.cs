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

            _data = data;

            if(data != null)
            {
                transform.localPosition = data.Pos;
            }

            EditObject?.Init(this);
            ActiveEditObject(false);
        }

        public override void ChainUpdate()
        {
            return;
        }

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

            state.Apply(this);

            State = state;
        }

        public void ActiveEditObject(bool active)
        {
            UIUtils.SetActive(EditObject?.transform, active);
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
            //var arrangeCmd = new Command.Remove(ObjectUId, transform.position);
            //arrangeCmd?.Execute();
        }

        void EditObject.IListener.Arrange()
        {
            var cmd = new Command.Arrange(ObjectUId, transform.position);
            cmd?.Execute();
        }
        #endregion
    }
}

