using System.Collections;
using System.Collections.Generic;
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
        [SerializeField]
        protected bool isWind = false;

        protected Collider2D _collider2D = null;
        protected Game.Type.EMaterial _eMaterial = Type.EMaterial.None;
        protected bool _spwaned = false;

        public ElementCollision ElementCollision { get; protected set; } = null;
        public ElementData ElementData { get; protected set; } = null;
        public Game.Element.State.BaseState State { get; protected set; } = null;

        public override void ChainUpdate()
        {
            base.ChainUpdate();
        }

        public void ActiveEdit(bool active)
        {
            UIUtils.SetActive(edit?.CanvasRectTm, active);
        }

        public void SetLocalPos(float x, float y, float z)
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

        protected void SetMaterial(Game.Type.EMaterial eMaterial)
        {
            if (_eMaterial == eMaterial)
                return;

            var material = GameSystem.ResourceManager.Instance?.GetMaterial(eMaterial);
            if (material == null)
                return;

            if (spriteRenderer?.material == null)
                return;


            if(eMaterial == Type.EMaterial.WindEffect)
            {
                material.SetFloat("_WindIntensity", Random.Range(13f, 16f));
                material.SetFloat("_WindSpeed", 1f);
            }

            spriteRenderer.material = material;

            _eMaterial = eMaterial;
        }

        protected void SetState(Game.Element.State.BaseState state)
        {
            if (State != null &&
                state != null)
            {
                if (State.CheckState(state.Type))
                    return;
            }

            State?.End();

            if (state is Game.Element.State.Edit)
            {
                SetMaterial(Game.Type.EMaterial.Outline);
                SetOutline(5f);
            }
            else
            {
                SetOutline(0);

                if (isWind)
                {
                    SetMaterial(Game.Type.EMaterial.WindEffect);
                }
            }

            state?.Apply(this);

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

        public void SetSpwaned(bool spwaned)
        {
            _spwaned = spwaned;
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

        protected virtual void SetSortingOrder(int order)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.sortingOrder = order;
        }

        protected void Arrange()
        {
            Command.Arrange.Execute(this, LocalPos);

            SetSortingOrder(-(int)LocalPos.y);

            SetSpwaned(false);
            ActiveEdit(false);
            SetState(null);
        }

        protected void Remove()
        {
            Command.Remove.Execute(this);

            SetSpwaned(true);
            ActiveEdit(false);
            SetState(null);
        }
    }
}
