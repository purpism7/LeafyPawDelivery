using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class AcquireHolder : Holder.Base
    {
        protected override string JsonFilePath => RootJsonFilePath + "/Info/";
        private string JsonFileName = "Acquire.json";

        public override void LoadInfo()
        {
            
        }
    }
}

