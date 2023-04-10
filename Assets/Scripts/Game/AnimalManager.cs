using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AnimalManager : Manager.Base
    {
        private List<Creature.Animal> _animalList = new();

        public override IEnumerator CoInit()
        {
            Debug.Log("AnimalManager CoInit");

            yield break;
        }

        public void ChainUpdate()
        {
            if (_animalList == null)
            {
                return;
            }

            foreach (var animal in _animalList)
            {
                animal?.ChainUpdate();
            }
        }

        public bool AddAnimal(Creature.Animal addAnimal)
        {
            if (addAnimal == null)
            {
                return false;
            }

            if (_animalList.Find(animal => animal.Id == addAnimal.Id) != null)
            {
                return false;
            }

            _animalList.Add(addAnimal);

            return true;
        }
    }
}

