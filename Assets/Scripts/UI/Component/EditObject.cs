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

            mainGameMgr.AddObjectToPlace(_data.ObjectId, _data.ObjectUId);
        }
    }
}

