using System.Collections;
using System.Collections.Generic;
using Game.Element.State;
using UnityEngine;

namespace Game
{
    public abstract class BaseElement : Game.Base, UI.Edit.IListener
    {
        [HideInInspector]
        public int UId = 0;

        [Header("Element")]
        [SerializeField]
        protected SpriteRenderer spriteRenderer = null;
        [SerializeField]
        protected Animator animator = null;
        [SerializeField]
        protected UI.Edit edit = null;

        [Tooltip("배치 중 다른 주민 / 꾸미기 요소와 겹쳐서 배치 가능한지 여부. (체크 시, 겹쳐서 배치 불가.)")]
        public bool IsOverlap = false;
        [Tooltip("배치 후 주민들이 통과 이동 가능 여부. (체크 시, 통과 이동 가능.)")]
        [SerializeField]
        private bool goPass = true;
        [SerializeField]
        protected bool isWind = false;

        protected Collider2D _collider2D = null;
        protected Game.Type.EMaterial _eMaterial = Type.EMaterial.None;
        protected bool _spwaned = false;
        protected List<Element.State.Base> _cachedStateList = null;

        public ElementCollision ElementCollision { get; protected set; } = null;
        public ElementData ElementData { get; protected set; } = null;
        public Game.Element.State.Base State { get; protected set; } = null;
        public bool IsMoving { get; private set; } = false;
        public bool GoPass { get { return goPass; } }

        public override void ChainUpdate()
        {
            base.ChainUpdate();
        }

        protected virtual void SetSortingOrder(int order)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.sortingOrder = order;
        }

        private void SetLocalPos(float x, float y, float z)
        {
            if (!transform)
                return;

            transform.localPosition = new Vector3(x, y, z);
        }

        public void SetLocalPos(Vector3 pos)
        {
            if (!transform)
                return;

            transform.localPosition = pos;
        }

        public void SetLocalPosZ(float z)
        {
            if (!transform)
                return;

            SetLocalPos(transform.localPosition.x, transform.localPosition.y, z);
        }

        public void AddRigidBody2D()
        {
            if (_collider2D == null)
                return;

            var rigidbody2D = _collider2D.gameObject.GetOrAddComponent<Rigidbody2D>();
            if (rigidbody2D == null)
                return;

            rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            rigidbody2D.useFullKinematicContacts = true;
        }

        public virtual void SetColor(Color color)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.color = color;
        }

        protected virtual void SetOutline(float width)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.material?.SetFloat("_Thickness", width);
        }

        protected void SetMaterial(Game.Type.EMaterial eMaterial, SpriteRenderer sprRenderer)
        {
            if (_eMaterial == eMaterial)
                return;

            var material = GameSystem.ResourceManager.Instance?.GetMaterial(eMaterial);
            if (material == null)
                return;

            if (sprRenderer?.material == null)
                return;

            if(eMaterial == Type.EMaterial.WindEffect)
            {
                material.SetFloat("_WindIntensity", Random.Range(13f, 16f));
                material.SetFloat("_WindSpeed", 1f);
            }

            sprRenderer.material = material;

            _eMaterial = eMaterial;
        }

        protected void SetState<T>(GameSystem.GameCameraController gameCameraCtr = null, GameSystem.IGrid iGrid = null) where T : Element.State.Base, new()
        {
            if (State != null)
            {
                if (State.CheckState(typeof(T)))
                    return;
            }

            if (_cachedStateList == null)
            {
                _cachedStateList = new();
                _cachedStateList.Clear();
            }
            
            Game.Element.State.Base state = null;
            Game.Element.State.Base findState = _cachedStateList.Find(cachedState => cachedState.GetType() == typeof(T));
            if (findState != null)
                state = findState;
            else
            {
                state = new T();
                state.Initialize(gameCameraCtr, iGrid);
                
                _cachedStateList?.Add(state);
            }
            
            State?.End();
            
            if (state is Game.Element.State.Edit)
            {
                SetMaterial(Game.Type.EMaterial.Outline, spriteRenderer);
                SetOutline(6f);
            }
            else
            {
                SetOutline(0);

                if (isWind)
                {
                    SetMaterial(Game.Type.EMaterial.WindEffect, spriteRenderer);
                }
            }

            state.Apply(this);

            State = state;
        }

        public void InteractableReturnBtn()
        {
            if (edit == null)
                return;

            edit.InteractableReturnBtn(!_spwaned);
        }

        public void InteractableArrangeBtn(bool interactable)
        {
            if (edit == null)
                return;

            edit.InteractableArrangeBtn(interactable);
        }

        public void EnableCollision(bool enable)
        {
            if (_collider2D == null)
                return;

            if (enable)
            {
                ElementCollision = _collider2D.gameObject.GetOrAddComponent<ElementCollision>();
                ElementCollision?.Initialize(this);
            }
            else
            {
                if(ElementCollision != null)
                {
                    Destroy(_collider2D.gameObject.GetComponent<ElementCollision>());
                }
                
                ElementCollision = null;
            }
        }

        public void SetSpawned(bool spwaned)
        {
            _spwaned = spwaned;
        }

        #region Edit
        protected void CreateEdit(Transform rootTm)
        {
            edit = new GameSystem.UICreator<UI.Edit, UI.Edit.Data>()
                .SetData(new UI.Edit.Data()
                {
                    Id = Id,
                    IListener = this,
                })
                .SetRooTm(rootTm)
                .Create();

            if(edit != null)
            {
                edit.transform.localPosition = Vector3.zero;
            }

            edit?.DeactivateEdit();
        }
        
        protected abstract void Return();

        protected virtual void Arrange()
        {
            Command.Arrange.Execute(this, LocalPos);

            SetSortingOrder(-(int)LocalPos.y);

            SetSpawned(false);
            edit?.DeactivateEdit();
            SetState<DeActive>();
        }

        protected virtual void Remove(bool refresh)
        {
            Command.Remove.Execute(this, refresh);

            SetSpawned(true);
            edit?.DeactivateEdit();
            SetState<DeActive>();
        }

        protected virtual void Conversation()
        {
            
        }
        
        protected virtual void Special()
        {
            
        }
        #endregion

        #region Edit.IListener
        void UI.Edit.IListener.Move(bool isMoving)
        {
            IsMoving = isMoving;

            _possibleTouchAction?.Invoke(isMoving);
        }

        void UI.Edit.IListener.Return()
        {
            Return();

            _touchEndAction?.Invoke();
            SetTouchAction();
        }

        void UI.Edit.IListener.Remove()
        {
            Remove(true);

            _touchEndAction?.Invoke();
            SetTouchAction();
        }

        void UI.Edit.IListener.Arrange()
        {
            Arrange();

            _touchEndAction?.Invoke();
            SetTouchAction();
        }

        void UI.Edit.IListener.Conversation()
        {
            Conversation();
        }

        void UI.Edit.IListener.Special()
        {
            Special();
        }
        #endregion
    }

    public abstract class BaseElement<T> : BaseElement where T : BaseData
    {
        protected const int SelectOrder = 9999;

        protected T _data = default(T);

        public virtual void Initialize(T data)
        {
            InternalInitialize(data);
        }

        public virtual IEnumerator CoInitialze(T data)
        {
            InternalInitialize(data);

            yield return null;
        }

        private void InternalInitialize(T data)
        {
            _data = data;

            if (_collider2D == null)
            {
                // if empty, get any collider2D from children
                _collider2D ??= GetComponentInChildren<Collider2D>();

                //_collider2D = GetComponentInChildren<PolygonCollider2D>();
                //if(_collider2D == null)
                //{
                //    _collider2D = GetComponentInChildren<CapsuleCollider2D>();
                //    if(_collider2D == null)
                //    {
                //        _collider2D = GetComponentInChildren<BoxCollider2D>();
                //    }
                //}
            }

            if (ElementCollision == null)
            {
                if(_collider2D != null)
                {
                    EnableCollision(false);
                }
            }
        }
    }
}
