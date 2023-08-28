using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class EditObject : UI.Base<EditObject.Data>
    {
        public class Data : BaseData
        {
            public int ObjectId = 0;
            public int ObjectUId = 0;
        }
        
        [SerializeField] private Image iconImg = null;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            SetIconImg();
        }
        
        private void SetIconImg()
        {
            if (_data == null)
                return;

            iconImg.sprite = GameUtils.GetShortIconSprite(Game.Type.EElement.Object, _data.ObjectId);
        }

        public void OnClick()
        {
            if(_data == null)
                return;

            var mainGameMgr = MainGameManager.Instance;
            if(mainGameMgr == null)
                return;

            Vector3 pos = Vector3.zero;
            var camera = mainGameMgr.GameCameraCtr?.GameCamera;
            if (camera)
            {
                pos = camera.transform.position + camera.transform.forward;
            }
            
            var objData = new Game.Object.Data()
            {
                ObjectId = _data.ObjectId,
                ObjectUId = _data.ObjectUId,
                Pos = pos,
            };
            
            var obj = new GameSystem.ObjectCreator<Game.Object, Game.Object.Data>()
                .SetData(objData)
                .SetId(_data.ObjectId)
                .SetRootTm(mainGameMgr.ObjectRootTm)
                .Create();

            mainGameMgr.AddObjectToPlace(obj);
        }
    }
}

