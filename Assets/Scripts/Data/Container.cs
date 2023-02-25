using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [ExecuteInEditMode]
    public class Container : MonoBehaviour
    {
        public List<Animal> AnimalDataList = new List<Animal>();

        public void AddAnimaData(Animal addAnimalData)
        {
            if(addAnimalData == null)
            {
                return;
            }

            foreach(var animalData in AnimalDataList)
            {              
                if(animalData == null)
                {
                    continue;
                }

                if(animalData.Id == addAnimalData.Id)
                {
                    return;
                }
            }

            AnimalDataList.Add(addAnimalData);
        }

        public Animal GetAnimal(int id)
        {
            return AnimalDataList?.Find(data => data?.Id == id);
        }
    }
}

