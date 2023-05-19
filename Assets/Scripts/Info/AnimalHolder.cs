using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class AnimalHolder : Holder.Base
    {
        protected override string JsonFilePath {
            get
            {
                return "Assets/Info/Animal.json";
            }
        }

        protected override void LoadInfo()
        {
            
        }
    }
}

