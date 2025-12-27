using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;

using UI;
using System;
using UI.WorldUI;
using static Game.Type;

namespace Game
{
    public interface IObject
    {
        bool IsActivate { get; }

        void Remove(bool refresh);

        Transform HiddenObjectRootTm { get; }
        bool CheckExistHiddenObject { get; }
        void ActivateHiddenObject();
        void DeactivateHiddenObject();
        bool IsHiddenObject { get; }

        int SortingOrder { get; }
        Vector3 LocalPos { get; }
    }

    public class Object : Game.BaseElement<Object.Data>, IObject
    {
        public class Data : BaseData
        {
            public int ObjectId = 0;
            public int ObjectUId = 0;
            public string ObjectUniqueID { get; private set; } = string.Empty;
            
            public Vector3 Pos = Vector3.zero;
            public ObjectType ObjectType { get; private set; } = ObjectType.None;

            public bool isHiddenObj = false;
            public int sortingOrder = 0;

            public Data WithObjectUniqueID(string uniqueID)
            {
                ObjectUniqueID = uniqueID;
                return this;
            }

            public Data WithObjectType(ObjectType objectType)
            {
                ObjectType = objectType;
                return this;
            }
        }

        #region Inspector
        [SerializeField]
        private int sortingOrderOffset = 0;
        [SerializeField]
        private Transform hiddenRootTm = null;
        [SerializeField] 
        private Transform arrivalPointTm = null;
        #endregion

        

        public int ObjectUId { get { return _data != null ? _data.ObjectUId : 0; } }

        public ObjectActController ObjectActCtr { get; private set; } = null;

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
                    sortingOrder = data.sortingOrder;
                else
                    CreateEdit(rootTm);

                SetSortingOrder(sortingOrder);
            }

            if (isWind)
                SetMaterial(Game.Type.EMaterial.WindEffect);

            ObjectActCtr = gameObject.GetOrAddComponent<ObjectActController>()?.Initialize(animator);
            
            edit?.Initialize(new Edit.Data()
            {
                IListener = this,
            });

            edit?.DeactivateEdit();
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

                SetState<Game.Element.State.Edit>(gameCameraCtr, iGrid);

                SetSortingOrder(SelectOrder);
                edit?.ActivateBottom();

                State?.Touch(TouchPhase.Began, null);

                //if(_bloomTimerWorldUI == null)
                //{
                //var data = new UI.WorldUI.BloomTimerWorldUI.Data
                //{

                //};

                //data.WithTargetTm(transform)
                //    .WithOffset(new Vector2(0, 120f));

                //_bloomTimerWorldUI = new GameSystem.ComponentCreator<UI.WorldUI.BloomTimerWorldUI, UI.WorldUI.BloomTimerWorldUI.Data>()
                //    .SetRootRectTm(UIManager.Instance?.WorldUIRootRectTr)
                //    .SetData(data)
                //    .Create();
                //}

                //_waterWorldUI?.Deactivate();

                return;
            }
            else
            {
                if (_data.isHiddenObj)
                {
                    Command.ObtainHiddenObject.Execute(this);
                    return;
                }

                SetState<Game.Element.State.Play>(gameCameraCtr, iGrid);
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

        protected override void Remove(bool refresh)
        {
            base.Remove(refresh);

            RemoveHiddenObject();
        }

        protected override void Arrange()
        {
            SetLocalPosZ(LocalPos.y * GameUtils.PosZOffset);

            _data.Pos = LocalPos;

            base.Arrange();
        }

        #region IObject
        void IObject.Remove(bool refresh)
        {
            Remove(refresh);
        }

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

