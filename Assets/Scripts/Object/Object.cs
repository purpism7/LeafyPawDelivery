using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Object : Game.Base<Object.Data>
    {
        public class Data : BaseData
        {
            public int ObjectUId = 0;
            public Vector3 Pos = Vector3.zero;
        }

        public override void Init(Data data)
        {
            base.Init(data);

            _data = data;

            if(data != null)
            {
                transform.position = data.Pos;
            }
        }

        public override void ChainUpdate()
        {
            return;
        }

        public override void OnTouch()
        {
            base.OnTouch();

            Debug.Log(name);
            Debug.Log(transform.position);
            GameSystem.GameManager.Instance?.ArrangeObject(_data.ObjectUId, transform.position);

        }
    }
}

