using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Component
{
    public class Edit : UI.Base<Edit.Data>
    {
        public class Data : BaseData
        {
            public int Id = 0;
        }

        public override void Init(Data data)
        {
            base.Init(data);
        }

        public void OnClick()
        {
            if(_data == null)
            {
                return;
            }

            var objData = new Game.Object.Data()
            {
                Id = 1,
            };

            new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                .SetData(objData)
                .SetId(1)
                .Create();
        }
    }
}

