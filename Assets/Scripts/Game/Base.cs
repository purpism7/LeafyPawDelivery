using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    public class BaseData
    {

    }

    public abstract class Common : MonoBehaviour, Sequencer.ITask
    {
        [SerializeField]
        protected Transform rootTm = null;

        protected bool _endTask = true;

        public virtual void Activate()
        {
            GameUtils.SetActive(rootTm, true);
        }

        public virtual void Deactivate()
        {
            GameUtils.SetActive(rootTm, false);
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
                return _endTask;
            }
        }
        #endregion

    }

    public abstract class Base : Common
    {
        public int Id = 0;

        protected System.Action _touchEndAction = null;

        public virtual void OnTouchBegan(Touch? touch, GameSystem.GameCameraController gameCameraCtr, GameSystem.IGrid iGrid) { }
        public virtual void OnTouch(Touch touch) { }
        public virtual void OnTouchEnded(Touch? touch, GameSystem.IGrid iGrid) { }

        public virtual void ChainUpdate() { }

        public Vector3 LocalPos
        {
            get
            {
                if (!transform)
                    return Vector3.zero;

                return transform.localPosition;
            }
        }

        public void SetTouchEndAction(System.Action touchEndAction)
        {
            _touchEndAction = touchEndAction;
        }
    }

    public abstract class Base<T> : Base where T : BaseData
    {
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

