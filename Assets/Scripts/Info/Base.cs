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
            //RootJsonFilePath = string.Format(RootJsonFilePath, GameSystem.Auth.ID);

            //Debug.Log("Holder Type = " + RootJsonFilePath);
        }

#if UNITY_EDITOR
        protected string RootJsonFilePath = "Assets/Info/";
#else
        protected string RootJsonFilePath = Application.persistentDataPath + "/Info/";
#endif
        
        protected abstract string JsonFilePath { get; }
        public abstract void LoadInfo();
    }
}

