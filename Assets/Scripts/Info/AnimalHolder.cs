using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class AnimalHolder : Holder.Base
    {
        protected override string JsonFilePath => RootJsonFilePath + "/Info/Animal.json";
        
        public List<Info.Animal> AnimalInfoList { get; private set; } = new();
        
        public override void LoadInfo()
        {
            AnimalInfoList.Clear();
            
            if (!System.IO.File.Exists(JsonFilePath))
                return;
            
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

        public bool AddAnimalInfo(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return false;

            if (GetAnimalInfo(animalInfo.Id) != null)
                return false;
            
            AnimalInfoList.Add(animalInfo);
            
            SaveInfo();

            return true;
        }
        
        public Info.Animal GetAnimalInfo(int animalId)
        {
            if(AnimalInfoList == null)
                return null;

            return AnimalInfoList.Find(animalInfo => animalInfo.Id == animalId);
        }
    }
}

