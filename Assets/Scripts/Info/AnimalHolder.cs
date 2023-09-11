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

            AddSkin(animalInfo, Game.Data.Const.AnimalBaseSkinId);
            
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

        #region Skin
        public void AddASkin(int id, int skinId)
        {
            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return;

            AddSkin(animalInfo, skinId);
        }

        private void AddSkin(Info.Animal animalInfo, int skinId)
        {
            if (animalInfo == null)
                return;

            if(animalInfo.SkinIdList == null)
            {
                animalInfo.SkinIdList = new();
                animalInfo.SkinIdList.Clear();
            }

            foreach(int animalSkinId in animalInfo.SkinIdList)
            {
                if(animalSkinId == skinId)
                    return;
            }

            animalInfo.SkinIdList.Add(skinId);

            SaveInfo();
        }

        public void ApplySkin(int id, int skinId)
        {
            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return;

            ApplySkin(animalInfo, skinId);
        }

        private void ApplySkin(Info.Animal animalInfo, int skinId)
        {
            var skinIdList = animalInfo?.SkinIdList;
            if (skinIdList == null)
                return;

            foreach(int animalSkinId in skinIdList)
            {
                if(animalSkinId == skinId)
                {

                }
            }
        }

        public bool CheckExist(int id, int skinId)
        {
            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return false;

            var skinIdList = animalInfo?.SkinIdList;
            if (skinIdList == null)
                return false;

            foreach (int animalSkinId in skinIdList)
            {
                if (animalSkinId == skinId)
                {
                    return true;
                }
            }

            return false;
        }

        public int GetCurrenctSkinId(int id)
        {
            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return 0;

            return animalInfo.SkinId;
        }
        #endregion

        public Info.Animal GetAnimalInfo(int id)
        {
            if(AnimalInfoList == null)
                return null;

            return AnimalInfoList.Find(animalInfo => animalInfo.Id == id);
        }
    }
}

