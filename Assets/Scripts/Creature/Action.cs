using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature
{
    [System.Serializable]
    public class Action
    { 
        public interface IListener<T> where T : Action
        {
            void StartAction(T t);
            void EndAction(T t);
        }

        public enum EState
        {
            None,

            Start,
            InProgress,
            End,
        }

        public class Data<T> where T : Action
        {
            public IListener<T> IListener = null;
            public Transform Tm = null;
            public Animator Animator = null;
            public EState EState = EState.None;
        }
    }
}
