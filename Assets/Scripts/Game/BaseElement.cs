using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace Game
{
    public abstract class BaseElement : Game.Base
    {
        [HideInInspector]
        public int UId = 0;

        [Header("Element")]
        [SerializeField]
        protected SpriteRenderer spriteRenderer = null;
        [SerializeField]
        protected UI.Edit edit = null;

        public bool IsOverlap = false;

        //protected PolygonCollider2D _polygonCollider2D = null;
        protected Collider2D _collider2D = null;

        public ElementCollision ElementCollision { get; protected set; } = null;
        public ElementData ElementData { get; protected set; } = null;
        public Game.Element.State.BaseState State { get; protected set; } = null;

        public override void ChainUpdate()
        {
            base.ChainUpdate();

            //State?.ChainUpdate();
        }

        public void ActiveEdit(bool active)
        {
            UIUtils.SetActive(edit?.CanvasRectTm, active);
        }

        //public void EnableCollider(bool enable)
        //{
        //    if (!IsActivate)
        //        return;

        //    if(_polygonCollider2D != null)
        //    {
        //        _polygonCollider2D.enabled = enable;
        //    }
        //}

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

        public void SetColor(Color color)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.color = color;
        }

        private void SetOutline(float width)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer?.material?.SetFloat("_Thickness", width);
        }

        protected void SetState(Game.Element.State.BaseState state)
        {
            if (State != null &&
                state != null)
            {
                if (State.CheckEqual(state))
                    return;
            }

            State?.End();

            if (state is Game.Element.State.Edit)
            {
                SetOutline(5f);
            }
            else
            {
                SetOutline(0);
            }

            state?.Apply(this);

            State = state;
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
                _collider2D = GetComponentInChildren<PolygonCollider2D>();
                if(_collider2D == null)
                {
                    _collider2D = GetComponentInChildren<CapsuleCollider2D>();
                    if(_collider2D == null)
                    {
                        _collider2D = GetComponentInChildren<BoxCollider2D>();
                    }
                }
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
