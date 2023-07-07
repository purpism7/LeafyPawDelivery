using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class AnimalHolder : Holder.Base
    {
        protected override string JsonFilePath => RootJsonFilePath + "/Info/";
        private string JsonFileName = "Animal.json";
        
        public List<Info.Animal> AnimalInfoList { get; private set; } = new();
        
        public override void LoadInfo()
        {
            AnimalInfoList.Clear();

            if(!System.IO.Directory.Exists(JsonFilePath))
            {
                System.IO.Directory.CreateDirectory(JsonFilePath);
            }

            var fullPath = JsonFilePath + JsonFileName;
            if (!System.IO.File.Exists(fullPath))
                return;
            
            var jsonString = System.IO.File.ReadAllText(fullPath);
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
            Debug.Log("jsonString = " + jsonString);
            System.IO.File.WriteAllText(JsonFilePath + JsonFileName , jsonString);
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

        public void RemoveAnimal(int id)
        {
            var animalData = AnimalContainer.Instance.GetData(id);
            if (animalData == null)
                return;

            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return;

            animalInfo.Pos = Vector3.zero;
            animalInfo.Arrangement = false;

            SaveInfo();
        }

        public void ArrangeAnimal(int id, Vector3 pos, int placeId)
        {
            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return;

            animalInfo.Pos = pos;
            animalInfo.Arrangement = true;

            SaveInfo();
        }

        public Info.Animal GetAnimalInfo(int id)
        {
            if(AnimalInfoList == null)
                return null;

            return AnimalInfoList.Find(animalInfo => animalInfo.Id == id);
        }
    }
}

