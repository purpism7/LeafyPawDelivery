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
            new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                .SetId(1)
                .Create();
        }
    }
}

