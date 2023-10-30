using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;

using UI;
using System;

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

        #region Inspector
        [SerializeField]
        private int sortingOrderOffset = 0;
        #endregion

        public int ObjectUId { get { return _data != null ? _data.ObjectUId : 0; } }
        public Game.Element.State.BaseState<Object> State { get; private set; } = null;

        private void OnDrawGizmos()
        {
            //if (Collider != null)
            //{
            //    var capsuleCollider = Collider as CapsuleCollider;

            //    if (capsuleCollider != null)
            //    {
            //        Gizmos.color = Color.blue;
            //        Gizmos.DrawWireSphere(capsuleCollider.center + transform.position, capsuleCollider.radius);
            //    }
            //}
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            ElementData = ObjectContainer.Instance?.GetData(Id);

            if (data != null)
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

        // object 최초 선택 시, 호출.
        public override void OnTouchBegan(Touch? touch, GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid)
        {
            base.OnTouchBegan(touch, gameCameraCtr, iGrid);

            Game.State.Base gameState = MainGameManager.Instance?.GameState;
            if (gameState == null)
                return;

            if(gameState.CheckState<Game.State.Edit>())
            {
                SetState(new Game.Element.State.Edit<Game.Object>()?.Create(gameCameraCtr, iGrid));

                SetSortingOrder(_selectOrder);
                ActiveEdit(true);
            }
            else
            {
                SetState(new Game.Element.State.Game<Game.Object>()?.Create(gameCameraCtr, iGrid));
            }
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            State?.Touch(touch);
        }

        private void SetState(Game.Element.State.BaseState<Game.Object> state)
        {
            if(State != null &&
               state != null) 
            {
                if (State.CheckEqual(state))
                    return;
            }

            if (state is Game.Element.State.Edit<Game.Object>)
            {
                StartEdit();
            }

            state?.Apply(this);

            State = state;
        }

        private void SetSortingOrder(int order)
        {
            if (spriteRenderer == null)
                return;

            order += sortingOrderOffset;

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

        //    var obj = collision.gameObject.GetComponent<Game.Object>();
        //    if (obj != null)
        //    {
        //        Debug.Log(obj.name);
        //    }
        //}
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
            EState_ = EState.Arrange;
            Command.Arrange.Execute(this, transform.localPosition);

            SetSortingOrder(-(int)transform.localPosition.y);

            ActiveEdit(false);
            SetState(null);
        }
        #endregion
    }
}

