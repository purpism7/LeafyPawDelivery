using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Creature;

namespace GameSystem
{
    public class GameManager : Singleton<GameManager>
    {
        public Game.AnimalManager AnimalMgr { get; private set; } = null;
        public Data.Container DataContainer { get; private set; } = null;

        public override IEnumerator CoInit()
        {
            AnimalMgr = new();

            DataContainer = FindObjectOfType<Data.Container>();

            yield return null;
        }

        private void Update()
        {
            AnimalMgr?.ChainUpdate();
        }

        public Animal AddAnimal(Animal animal)
        {
            if(animal == null)
            {
                return null;
            }

            var animalData = DataContainer.GetAnimal(animal.Id);
            if(animalData == null)
            {
                Debug.LogError(name + " = No Animal Data");
                return null;
            }

            if (!AnimalMgr.AddAnimal(animal))
            {
                return null;
            }

            return animal;
        }
    }
}

