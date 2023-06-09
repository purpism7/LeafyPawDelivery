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

        public void AddAnimalInfo(Info.Animal animalInfo)
        {
            _animalHolder?.AddAnimalInfo(animalInfo);
        }

        public bool CheckExist(int animalId)
        {
            if (AnimalInfoList == null)
                return false;
            
            return GetAnimalInfo(animalId) != null;
        }

        public Info.Animal GetAnimalInfo(int animalId)
        {
            return _animalHolder?.GetAnimalInfo(animalId);
        }
    }
}

