﻿using System.Collections;
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
        void ActivateHiddenObject();
        void DeactivateHiddenObject();
        bool IsHiddenObject { get; }

        int SortingOrder { get; }
        Vector3 LocalPos { get; }

        bool IsActivate { get; }
    }

    public class Object : Game.BaseElement<Object.Data>, IObject
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

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            ElementData = ObjectContainer.Instance?.GetData(Id);

            if (data != null)
            {
                UId = ObjectUId;

                SetPos();

                int sortingOrder = -(int)LocalPos.y;
                if(data.isHiddenObj)
                {
                    sortingOrder = data.sortingOrder;
                }
                else
                {
                    CreateEdit(rootTm);
                }

                SetSortingOrder(sortingOrder);
            }

            if (isWind)
            {
                SetMaterial(Game.Type.EMaterial.WindEffect);
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

                State?.Touch(TouchPhase.Began, null);

                return;
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
                State?.Touch(touch.Value.phase, touch.Value);
            }
        }

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            State?.Touch(touch.phase, touch);
        }

        private void SetPos()
        {
            if (_data == null)
                return;

            transform.localPosition = _data.Pos;
        }

        protected override void SetSortingOrder(int order)
        {
            if (spriteRenderer == null)
                return;

            order += sortingOrderOffset;

            base.SetSortingOrder(order);

            SetSortingOrderHiddenObject(order);
        }

        private void RemoveHiddenObject()
        {
            if (!CheckExistHiddenObject)
                return;

            while(hiddenRootTm.childCount > 0)
            {
                DestroyImmediate(hiddenRootTm.GetChild(0).gameObject);
            }
        }

        private void SetSortingOrderHiddenObject(int order)
        {
            if (!CheckExistHiddenObject)
                return;

            for(int i = 0; i < hiddenRootTm.childCount; ++i)
            {
                var childTm = hiddenRootTm.GetChild(i);
                if (!childTm)
                    continue;

                var hiddenObj = childTm.GetComponent<Object>();
                if (hiddenObj == null)
                    continue;

                hiddenObj.SetSortingOrder(order - 1);
            }
        }

        public void Reset(int uId, Vector3 pos)
        {
            UId = uId;

            if (_data == null)
                return;

            _data.Pos = pos;
        }

        protected override void Return()
        {
            if (_spwaned)
                return;

            SetLocalPos(_data.Pos);

            Arrange();
        }

        protected override void Remove()
        {
            base.Remove();

            RemoveHiddenObject();
        }

        protected override void Arrange()
        {
            SetLocalPosZ(LocalPos.y * GameUtils.PosZOffset);

            _data.Pos = LocalPos;

            base.Arrange();
        }

        #region IObject
        Transform IObject.HiddenObjectRootTm
        {
            get
            {
                return hiddenRootTm;
            }
        }

        public bool CheckExistHiddenObject
        {
            get
            {
                if (!hiddenRootTm)
                    return false;

                return hiddenRootTm.childCount > 0;
            }
        }

        void IObject.ActivateHiddenObject()
        {
            GameUtils.SetActive(hiddenRootTm, true);
        }

        void IObject.DeactivateHiddenObject()
        {
            GameUtils.SetActive(hiddenRootTm, false);
        }

        bool IObject.IsHiddenObject
        {
            get
            {
                if (_data == null)
                    return false;

                return _data.isHiddenObj;
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

        Vector3 IObject.LocalPos
        {
            get
            {
                return LocalPos;
            }
        }
        #endregion
    }
}

