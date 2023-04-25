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
        public int Id;
        public Game.State.Element ElementState { get; protected set; } = null;
        public EState EState_ { get; protected set; } = EState.None;

        public virtual void OnTouchBegan(Camera gameCamera, GameSystem.Grid grid)
        {

        }

        public virtual void OnTouch(Touch touch)
        {

        }

        public abstract void ChainUpdate();
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

