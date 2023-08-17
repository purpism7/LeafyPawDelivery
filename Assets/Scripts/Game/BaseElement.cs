using System.Collections;
using System.Collections.Generic;
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

        public Type.EElement EElement { get; protected set; } = Type.EElement.None;

        public void ActiveEdit(bool active)
        {
            UIUtils.SetActive(edit?.CanvasRectTm, active);
        }

        public void SetOutline(float width)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer?.material?.SetFloat("_Thickness", width);
        }
    }

    public abstract class BaseElement<T> : BaseElement where T : BaseData
    {
        readonly protected int _selectOrder = 9999;

        protected T _data = default(T);

        public virtual void Initialize(T data)
        {
            _data = data;
        }

        public virtual IEnumerator CoInitialze(T data)
        {
            _data = data;

            yield return null;
        }
    }
}
