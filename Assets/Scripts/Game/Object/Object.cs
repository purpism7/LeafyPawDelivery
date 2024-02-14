using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;

using UI;
using System;

namespace Game
{
    public interface IObject
    {
        Transform HiddenObjectRootTm { get; }
        bool CheckExistHiddenObject { get; }
        int SortingOrder { get; }
    }

    public class Object : Game.BaseElement<Object.Data>, UI.Edit.IListener, IObject
    {
        public class Data : BaseData
        {
            public int ObjectId = 0;
            public int ObjectUId = 0;
            public Vector3 Pos = Vector3.zero;

            public bool isHiddenObj = false;
            public int sortingOrder = 0;
        }

        #region Inspector
        [SerializeField]
        private int sortingOrderOffset = 0;
        [SerializeField]
        private Transform hiddenRootTm = null;
        #endregion

        public int ObjectUId { get { return _data != null ? _data.ObjectUId : 0; } }

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

                SetPos();

                int sortingOrder = -(int)transform.localPosition.y;
                if(data.isHiddenObj)
                {
                    sortingOrder = data.sortingOrder;
                }

                SetSortingOrder(sortingOrder);
            }

            edit?.Initialize(new Edit.Data()
            {
                IListener = this,
            });

            ActiveEdit(false);
        }

        public override void Activate()
        {
            base.Activate();

            SetPos();
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
                if (_data.isHiddenObj)
                    return;

                var editState = gameState.Get<Game.State.Edit>();
                if (editState != null &&
                    editState.CheckIsEditElement(this))
                    return;

                SetState(new Game.Element.State.Edit()?.Initialize(gameCameraCtr, iGrid));

                SetSortingOrder(SelectOrder);
                ActiveEdit(true);
            }
            else
            {
                if (_data.isHiddenObj)
                {
                    Command.ObtainHiddenObject.Execute(this);

                    return;
                }

                SetState(new Game.Element.State.Play()?.Initialize(gameCameraCtr, iGrid));
            }

            if(touch != null)
            {
                State?.Touch(touch.Value);
            }
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            State?.Touch(touch);
        }

        private void SetPos()
        {
            if (_data == null)
                return;

            transform.localPosition = _data.Pos;
        }

        private void SetSortingOrder(int order)
        {
            if (spriteRenderer == null)
                return;

            order += sortingOrderOffset;

            spriteRenderer.sortingOrder = order;
        }

        public void Reset(int uId, Vector3 pos)
        {
            UId = uId;

            if (_data == null)
                return;

            _data.Pos = pos;
        }

        #region IObject
        Transform IObject.HiddenObjectRootTm
        {
            get
            {
                return hiddenRootTm;
            }
        }

        bool IObject.CheckExistHiddenObject
        {
            get
            {
                if (!hiddenRootTm)
                    return false;

                return hiddenRootTm.childCount > 0;
            }
        }

        int IObject.SortingOrder
        {
            get
            {
                if (spriteRenderer == null)
                    return -9999;

                return spriteRenderer.sortingOrder;
            }
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
            Command.Arrange.Execute(this, transform.localPosition);

            SetSortingOrder(-(int)transform.localPosition.y);

            ActiveEdit(false);
            SetState(null);
        }
        #endregion
    }
}

