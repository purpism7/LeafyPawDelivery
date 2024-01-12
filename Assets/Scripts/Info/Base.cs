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
            RootJsonFilePath = string.Format(RootJsonFilePath, GameSystem.Auth.ID);

            Debug.Log("Holder Type = " + RootJsonFilePath);
        }

#if UNITY_EDITOR
        protected string RootJsonFilePath = "Assets/Info/{0}/";
#else
        protected string RootJsonFilePath = Application.persistentDataPath + "/Info/{0}/";
#endif
        
        protected abstract string JsonFilePath { get; }
        public abstract void LoadInfo();
    }
}

