using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AnimalManager : Manager.Base<AnimalManager.Data>
    {
        public class Data : Game.Manager.BaseData
        {
            public int PlaceId = 0;
        }
        
        private Info.AnimalHolder _animalHolder = new();

        public  List<Info.Animal> AnimalInfoList => _animalHolder?.AnimalInfoList;

        public override IEnumerator CoInit(Data data)
        {
            Debug.Log("AnimalManager CoInit");

            yield break;
        }

        public void AddAnimal(Info.Animal animalInfo)
        {
            _animalHolder?.AddAnimal(animalInfo);
        }

        public bool CheckExist(int id)
        {
            if (AnimalInfoList == null)
                return false;
            
            return _animalHolder.GetAnimalInfo(id) != null;
        }
    }
}

