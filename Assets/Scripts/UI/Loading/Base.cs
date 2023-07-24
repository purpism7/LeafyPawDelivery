using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Load
{
    public abstract class Base : MonoBehaviour
    {
        public abstract string SceneName { get; }
        public abstract bool ActiveLoading { get; }

        public Camera MainCamera { get; protected set; } = null;
    }
}

