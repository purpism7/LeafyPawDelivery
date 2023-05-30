using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace UI.Component
{
    public class EditObject : UI.Base<EditObject.Data>
    {
        public class Data : BaseData
        {
            public int ObjectId = 0;
            public int ObjectUId = 0;
        }

        [SerializeField]
        private TMPro.TextMeshProUGUI idTMP;

        public override void Init(Data data)
        {
            base.Init(data);

            idTMP?.SetText(data.ObjectId.ToString());
        }

        public void OnClick()
        {
            if(_data == null)
                return;

            var gameMgr = GameManager.Instance;
            if(gameMgr == null)
                return;

            var objData = new Game.Object.Data()
            {
                ObjectUId = _data.ObjectUId,
            };

            var obj = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                .SetData(objData)
                .SetId(_data.ObjectId)
                .SetRootTm(gameMgr.ObjectRootTm)
                .Create();

            gameMgr.AddObjectToPlace(obj);
        }
    }
}

