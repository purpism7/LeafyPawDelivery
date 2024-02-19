using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Info.Holder
{
    public abstract class Base
    {
        protected Base()
        {
            //LoadInfo();
            //RootJsonFilePath = string.Format(RootJsonFilePath, GameSystem.Auth.ID);
            //RootJsonFilePath = Application.persistentDataPath;
            //Debug.Log("Holder Type = " + RootJsonFilePath);
        }

//#if UNITY_EDITOR
        //protected string RootJsonFilePath = "Assets";
//#else
        protected string RootJsonFilePath = string.Empty;
//#endif

        protected abstract string JsonFilePath { get; }
        public abstract void LoadInfo();
    }
}

