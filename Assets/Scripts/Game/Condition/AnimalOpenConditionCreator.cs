using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

namespace GameSystem
{
    public class AnimalOpenConditionCreator : BaseCreator<AnimalOpenCondition>
    {
        AnimalOpenCondition.Data _data = new();

        public AnimalOpenConditionCreator SetAnimalId(int animalId)
        {
            _data.AnimalId = animalId;

            return this;
        }

        public AnimalOpenConditionCreator SetOpenCondition(GameData.OpenCondition openCondition)
        {
            _data.OpenCondition = openCondition;

            return this;
        }

        public AnimalOpenConditionCreator SetOpenAction(System.Action<int> openAction)
        {
            _data.OpenAction = openAction;

            return this;
        }

        public override AnimalOpenCondition Create()
        {
            var animalOpenCondition = new AnimalOpenCondition();
            animalOpenCondition?.Init(_data);

            return animalOpenCondition;
        }
    }
}
 
