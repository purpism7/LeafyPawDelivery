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

    public abstract class Common : MonoBehaviour, Sequencer.ITask
    {
        [SerializeField]
        private Transform rootTm = null;

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

        #region Sequencer.ITask
        public virtual void Begin()
        {

        }

        public virtual bool End
        {
            get
            {
                return true;
            }
        }
        #endregion

    }

    public abstract class Base : Common
    {
        [SerializeField]
        protected UI.Edit edit = null;

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


        public virtual void ChainUpdate()
        {
            
        }

        public void ActiveEdit(bool active)
        {
            UIUtils.SetActive(edit?.CanvasRectTm, active);
        }
    }

    public abstract class Base<T> : Base where T : BaseData
    {
        readonly protected int _selectOrder = 500;

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

