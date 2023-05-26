using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace UI.Component
{
    public class EditAnimal : UI.Base<EditAnimal.Data>
    {
        public class Data : BaseData
        {
            public Animal AnimalData = null;
        }

        [SerializeField]
        private TMPro.TextMeshProUGUI idTMP;

        public override void Init(Data data)
        {
            base.Init(data);

            idTMP?.SetText(data.AnimalData.Name.ToString());
        }

        public void OnClick()
        {
            if(_data == null)
            {
                return;
            }

            var gameMgr = GameManager.Instance;
            if(gameMgr == null)
            {
                return;
            }

            // var objData = new Game.Object.Data()
            // {
            //     ObjectUId = _data.ObjectUId,
            // };
            //
            // var obj = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
            //     .SetData(objData)
            //     .SetId(_data.ObjectId)
            //     .SetRootTm(gameMgr.ObjectRootTm)
            //     .Create();

            // gameMgr.StartEditAction?.Invoke(obj);
        }
    }
}