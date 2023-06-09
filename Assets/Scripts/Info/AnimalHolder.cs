using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class AnimalHolder : Holder.Base
    {
        protected override string JsonFilePath => "Assets/Info/Animal.json";

        public List<Info.Animal> AnimalInfoList { get; private set; } = new();
        
        protected override void LoadInfo()
        {
            AnimalInfoList.Clear();
            
            if (!System.IO.File.Exists(JsonFilePath))
            {
                return;
            }
            
            var jsonString = System.IO.File.ReadAllText(JsonFilePath);
            var animalInfos = JsonHelper.FromJson<Info.Animal>(jsonString);
            if(animalInfos != null)
            {
                AnimalInfoList.AddRange(animalInfos);
            }
        }

        private void SaveInfo()
        {
            if(AnimalInfoList == null)
                return;

            var jsonString = JsonHelper.ToJson(AnimalInfoList.ToArray());
   
            System.IO.File.WriteAllText(JsonFilePath, jsonString);
        }

        public void AddAnimalInfo(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return;

            if (GetAnimalInfo(animalInfo.Id) != null)
                return;
            
            AnimalInfoList.Add(animalInfo);
            
            SaveInfo();
        }
        
        public Info.Animal GetAnimalInfo(int animalId)
        {
            if(AnimalInfoList == null)
                return null;

            return AnimalInfoList.Find(animalInfo => animalInfo.Id == animalId);
        }
    }
}

