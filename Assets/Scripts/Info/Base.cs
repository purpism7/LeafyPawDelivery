using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info.Holder
{
    public abstract class Base
    {
        protected Base()
        {
            //LoadInfo();
        }

#if UNITY_EDITOR
        protected string RootJsonFilePath = "Assets";
#else
        protected string RootJsonFilePath = Application.persistentDataPath;
#endif
        
        protected abstract string JsonFilePath { get; }
        public abstract void LoadInfo();
    }
}

