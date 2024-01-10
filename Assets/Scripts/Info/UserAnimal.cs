using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public partial class User
    {
        [Serializable]
        public class Animal
        {
            public int id = 0;
            public List<int> skinIdList = new();
        }

        [SerializeField]
        private List<Animal> animalList = new();

        public List<Animal> AnimalList { get { return animalList; } }

        #region Animal
        public void AddAnimal(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return;

            if (CheckExistAnimal(animalInfo.Id))
                return;

            animalList.Add(new Animal()
            {
                id = animalInfo.Id,
            });

            AddAnimalSkin(animalInfo.Id, animalInfo.SkinId);
        }

        public void AddAnimalSkin(int id, int skinId)
        {
            if (!CheckExistAnimal(id))
                return;

            var animal = GetAnimal(id);
            if (animal == null)
                return;

            if(animal.skinIdList == null)
            {
                animal.skinIdList = new();
                animal.skinIdList.Clear();

                animal.skinIdList.Add(skinId);
            }
            else
            {
                if (animal.skinIdList.FindIndex(findSkindId => findSkindId == skinId) >= 0)
                    return;

                animal.skinIdList.Add(skinId);
            }
        }

        public Animal GetAnimal(int id)
        {
            if (animalList == null)
                return null;

            return animalList.Find(animal => animal.id == id);
        }

        private bool CheckExistAnimal(int id)
        {
            if (animalList == null)
                return false;

            return GetAnimal(id) != null;
        }
        #endregion
    }
}
