using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class AnimalManager : Manager.Base<AnimalManager.Data>
    {
        public class Data : Game.Manager.BaseData
        {
            public int PlaceId = 0;
        }
        
        private Data _data = null;
        private Info.AnimalHolder _animalHolder = new();

        public  List<Info.Animal> AnimalInfoList => _animalHolder?.AnimalInfoList;
        public UnityEvent<Info.Animal> Listener { get; private set; } = new();

        protected override void Initialize()
        {
            Debug.Log("AnimalManager Initialize");

            Listener = new UnityEvent<Info.Animal>();
            Listener.RemoveAllListeners();
        }

        public override IEnumerator CoInit(Data data)
        {
            _data = data;
            
            _animalHolder?.LoadInfo();

            Debug.Log("AnimalManager CoInit");
            
            yield break;
        }

        public void AddAnimalInfo(Info.Animal animalInfo)
        {
            if (_animalHolder == null)
                return;
            
            if (_animalHolder.AddAnimalInfo(animalInfo))
            {
                Listener?.Invoke(animalInfo);
            }
        }

        public void AddAnimalInfo(int animalId)
        {
            if (_animalHolder == null)
                return;
            
            var animalInfo = new Info.Animal()
            {
                Id = animalId,
            };

            if (_animalHolder.AddAnimalInfo(animalInfo))
            {
                Listener?.Invoke(animalInfo);
            }
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

