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
        [SerializeField]
        protected SpriteRenderer spriteRenderer = null;
        [SerializeField]
        protected UI.Edit edit = null;

        private CapsuleCollider _collider = null;

        public ElementData ElementData { get; protected set; } = null;
        public int SortingOrder { get { return spriteRenderer != null ? spriteRenderer.sortingOrder : 1; } }

        public void ActiveEdit(bool active)
        {
            UIUtils.SetActive(edit?.CanvasRectTm, active);
        }

        public void EnableCollider(bool enable)
        {
            if (_collider == null)
            {
                if (spriteRenderer == null)
                    return;

                _collider = spriteRenderer.GetComponent<CapsuleCollider>();
            }

            _collider.enabled = enable;
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

            MainGameManager.Instance?.placeMgr?.ActivityPlace?.EnableCollider(false);
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
            MainGameManager.Instance?.placeMgr?.ActivityPlace?.EnableCollider(true);
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
