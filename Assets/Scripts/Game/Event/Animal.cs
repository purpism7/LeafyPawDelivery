using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Event
{
    public class Animal : Base
    {
        public override void Initialize()
        {
            
        }

        public override void Starter(System.Action endAction)
        {
            var animalMgr = MainGameManager.Get<AnimalManager>();
            if (animalMgr == null)
                return;

            var animalOpenConidtionDatas = AnimalOpenConditionContainer.Instance?.Datas;
            if (animalOpenConidtionDatas == null)
                return;

            var animalContainer = AnimalContainer.Instance;

            foreach (var data in animalOpenConidtionDatas)
            {
                if (data == null)
                    continue;

                var animalData = animalContainer?.GetData(data.Id);
                if (animalData != null &&
                    animalData.PlaceId != GameUtils.ActivityPlaceId)
                    continue;

                if (animalMgr.CheckExist(data.Id))
                    continue;

                if (data.eType == OpenConditionData.EType.Starter)
                {
                    Sequencer.EnqueueTask(
                        () =>
                        {
                            var popup = new GameSystem.PopupCreator<UI.Obtain, UI.Obtain.Data>()
                                .SetData(new UI.Obtain.Data()
                                {
                                    EElement = Type.EElement.Animal,
                                    Id = data.Id,
                                    ClickAction = () =>
                                    {
                                        endAction?.Invoke();
                                    },
                                })
                                .SetCoInit(true)
                                .SetReInitialize(true)
                                .Create();

                            return popup;
                        });

                    animalMgr.Add(data.Id);
                }
            }
        }

        public override void Emit<AnimalData>(AnimalData animalData)
        {
            switch(animalData)
            {
                case AddAnimalData data:
                    {
                        break;
                    }
            }
        }
    }
}

