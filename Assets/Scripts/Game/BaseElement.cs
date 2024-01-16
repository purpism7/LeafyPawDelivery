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

        public Collider Collider = null;
        public Collider2D Collider2D = null;

        public ElementData ElementData { get; protected set; } = null;
        //public int SortingOrder { get { return spriteRenderer != null ? spriteRenderer.sortingOrder : 1; } }

        public void ActiveEdit(bool active)
        {
            UIUtils.SetActive(edit?.CanvasRectTm, active);
        }

        public void EnableCollider(bool enable)
        {
            if (!IsActivate)
                return;

            SetCollider<CapsuleCollider>();
            SetCollider<BoxCollider>();
            SetCollider<SphereCollider>();

            if(Collider != null)
            {
                Collider.enabled = enable;
            }
        }

        protected void SetCollider<T>() where T : Collider
        {
            if (Collider != null)
                return;

            Collider = transform.GetComponentInChildren<T>();
        }

        public void SetColor(Color color)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.color = color;
        }

        protected void SetOutline(float width)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer?.material?.SetFloat("_Thickness", width);
        }

        protected void StartEdit()
        {
            EState_ = EState.Edit;

            SetOutline(5f);

            MainGameManager.Get<Game.PlaceManager>()?.ActivityPlace?.EnableCollider(false);
            EnableCollider(true);

            Game.UIManager.Instance?.Bottom?.DeactivateEditList();
        }

        public void EndEdit()
        {
            EState_ = EState.None;

            SetOutline(0);

            var eTab = Game.Type.ETab.Animal;
            if (ElementData != null)
            {
                eTab = ElementData.EElement == Game.Type.EElement.Animal? Game.Type.ETab.Animal : Game.Type.ETab.Object;
            }
             
            Game.UIManager.Instance?.Bottom?.ActivateEditList(eTab);
            MainGameManager.Get<Game.PlaceManager>()?.ActivityPlace?.EnableCollider(true);
        }

        public void InteractableArrangeBtn(bool interactable)
        {
            if (edit == null)
                return;

            edit.InteractableArrangeBtn(interactable);
        }
    }

    public abstract class BaseElement<T> : BaseElement where T : BaseData
    {
        readonly protected int _selectOrder = 9999;

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
        }
    }
}
