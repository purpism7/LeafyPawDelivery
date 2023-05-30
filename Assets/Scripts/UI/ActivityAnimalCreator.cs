using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI;

namespace GameSystem
{
    public class ActivityAnimalCreator : BaseCreator<ActivityAnimal>
    {
        private int _animalId = 0;
        private string _animalName = string.Empty;
        private RectTransform _rootRectTm = null;
        private ActivityAnimalManager.SelectActivityAnimalDelegate _selectActivityAnimalDel = null;
  
        public ActivityAnimalCreator SetAnimalId(int animalId)
        {
            _animalId = animalId;

            return this;
        }

        public ActivityAnimalCreator SetAnimalName(string animalName)
        {
            _animalName = animalName;

            return this;
        }

        public ActivityAnimalCreator SetRoot(RectTransform rootRectTm)
        {
            _rootRectTm = rootRectTm;

            return this;
        }

        public ActivityAnimalCreator SetSelectActivityAnimal(ActivityAnimalManager.SelectActivityAnimalDelegate del)
        {
            _selectActivityAnimalDel = del;

            return this;
        }

        public override ActivityAnimal Create()
        {
            var activityAnimal = ResourceManager.Instance.InstantiateUI<ActivityAnimal>(_rootRectTm);
            if(activityAnimal == null)
            {
                return null;
            }

            activityAnimal.Initialize(new ActivityAnimal.Data()
            {
                AnimalId = _animalId,
                AnimalName = _animalName,
                SelectActivityAnimalDel = _selectActivityAnimalDel,
            });

            return activityAnimal;
        }
    }
}

