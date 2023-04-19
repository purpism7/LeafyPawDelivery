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

        public int ObjectUId { get { return _data != null ? _data.ObjectUId : 0; } }
        public ObjectState State { get; private set; } = null;

        public override void Init(Data data)
        {
            base.Init(data);

            _data = data;

            if(data != null)
            {
                transform.localPosition = data.Pos;
            }
        }

        public override void ChainUpdate()
        {
            return;
        }

        public void Apply(ObjectState state)
        {
            if(state == null)
            {
                return;
            }

            state.Apply(this);

            State = state;
        }


        private void OnCollisionStay(Collision collision)
        {
            Debug.Log("OnCollisionStay = " + collision.gameObject.name);
        }
    }
}

