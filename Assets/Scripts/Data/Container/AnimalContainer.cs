using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class AnimalContainer : BaseContainer<Animal>
    {
        public override void Init(string json)
        {
            base.Init(json);
        }
    }
}
