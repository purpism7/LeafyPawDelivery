using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum EState
    {
        None,

        Edit,
        Remove,
        Arrange,
    }

    public class BaseData
    {

    }

    public abstract class Base : MonoBehaviour
    {
        [SerializeField]
        private Transform rootTm = null;

        public int Id = 0;
        [HideInInspector]
        public int UId = 0;

        public EState EState_ { get; protected set; } = EState.None;

        public virtual void OnTouchBegan(Camera gameCamera, GameSystem.Grid grid)
        {

        }

        public virtual void OnTouch(Touch touch)
        {

        }

        public virtual void Activate()
        {
            UIUtils.SetActive(rootTm, true);
        }

        public virtual void Deactivate()
        {
            UIUtils.SetActive(rootTm, false);
        }
        
        public bool IsActivate
        {
            get
            {
                if (!rootTm)
                    return false;
            
                return rootTm.gameObject.activeSelf;
            }
        }

        public virtual void ChainUpdate()
        {
            
        }
    }

    public abstract class Base<T> : Base where T : BaseData
    {
        protected T _data = default(T);

        public virtual void Init(T data)
        {
            _data = data;
        }

        public virtual IEnumerator CoInit(T data)
        {
            _data = data;

            yield return null;
        }
    }
}

