using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Object : Game.Base<Object.Data>
    {
        public class Data : BaseData
        {

        }

        public override void Init(Data data)
        {
            base.Init(data);

            _data = data;
        }

        public override void ChainUpdate()
        {
            return;
        }

        public override void OnTouch()
        {
            base.OnTouch();

            Debug.Log(name);
        }
    }
}

