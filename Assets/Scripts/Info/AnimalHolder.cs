using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class AnimalHolder : Holder.Base
    {
        protected override string JsonFilePath {
            get
            {
                return "Assets/Info/Animal.json";
            }
        }

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
    }
}

