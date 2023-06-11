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

        protected abstract string JsonFilePath { get; }
        public abstract void LoadInfo();
    }
}

