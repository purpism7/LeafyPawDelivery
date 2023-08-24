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

            iconImg.sprite = GameUtils.GetShortIconSprite(Type.EElement.Animal, _data.AnimalData.Id);
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

            Vector3 pos = Vector3.zero;
            var gameCameraCtr = mainGameMgr.GameCameraCtr;
            if (gameCameraCtr != null)
            {
                pos = gameCameraCtr.Center;
            }

            var animal = new GameSystem.AnimalCreator()
                .SetAnimalId(_data.AnimalData.Id)
                .SetPos(pos)
                .Create();

            mainGameMgr.AddAnimalToPlace(animal);
        }
    }
}