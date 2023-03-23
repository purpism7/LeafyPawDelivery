using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public interface IState
    {
        System.Type Type { get; }
        bool CheckControlCamera { get; }
    }

    public abstract class Base : IState
    {
        System.Type IState.Type
        {
            get
            {
                return GetType();
            }
        }

        public abstract bool CheckControlCamera { get; }
    }
}

