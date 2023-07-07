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

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            idTMP?.SetText(data.AnimalData.Name.ToString());
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

            var animal = new GameSystem.AnimalCreator()
                .SetAnimalId(_data.AnimalData.Id)
                .Create();

            mainGameMgr.AddAnimalToPlace(animal);
        }
    }
}