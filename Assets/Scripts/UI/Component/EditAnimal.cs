using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class EditAnimal : UI.Base<EditAnimal.Data>
    {
        public class Data : BaseData
        {
            public Animal AnimalData = null;
            public int SkinId = 0;
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

            if (_data.AnimalData == null)
                return;

            iconImg.sprite = GameUtils.GetShortIconSprite(Game.Type.EElement.Animal, _data.AnimalData.Id);
        }

        public void OnClick()
        {
            if(_data == null)
                return;

            if (_data.AnimalData == null)
                return;

            var mainGameMgr = MainGameManager.Instance;
            if(mainGameMgr == null)
                return;

            mainGameMgr.AddAnimalToPlace(_data.AnimalData.Id, _data.SkinId);
        }
    }
}