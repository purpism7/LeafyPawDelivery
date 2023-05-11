using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Load
{
    public abstract class Base : MonoBehaviour
    {
        public abstract string SceneName { get; }
        public abstract void Create(RectTransform rootRectTm);
    }
}

