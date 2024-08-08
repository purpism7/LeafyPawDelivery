using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Security.Policy;

namespace Info
{
    public class AnimalHolder : Holder.Base
    {
        protected override string JsonFilePath => Path.Combine(RootJsonFilePath, JsonFileName);
        private string JsonFileName = "Animal.txt";

        private const string _secretKey = "hAnkyUlAnimAl";

        public List<Info.Animal> AnimalInfoList { get; private set; } = new();

        public override void LoadInfo()
        {
            RootJsonFilePath = Utility.GetInfoPath();

            AnimalInfoList.Clear();

            List<Info.Animal> animalInfoList = null;

            if (System.IO.File.Exists(JsonFilePath))
            {
                var decodeStr = System.IO.File.ReadAllText(JsonFilePath);
                var jsonStr = decodeStr.Decrypt(_secretKey);

                animalInfoList = JsonHelper.FromJson<Info.Animal>(jsonStr).ToList();
            }
            
            var user = Info.UserManager.Instance?.User;
            if (user == null)
                return;

            var animalList = user.AnimalList;
            foreach (var animal in animalList)
            {
                if (animal == null)
                    continue;

                Info.Animal animalInfo = null;

                if(animalInfoList != null)
                {
                    animalInfo = animalInfoList.Find(findAnimalInfo => findAnimalInfo != null && findAnimalInfo.Id == animal.id);
                }

                if(animalInfo == null)
                {
                    animalInfo = new Animal()
                    {
                        Id = animal.id,
                        SkinId = Games.Data.Const.AnimalBaseSkinId,
                        SkinIdList = animal.skinIdList,
                    };
                }
                else
                {
                    animalInfo.SkinIdList = animal.skinIdList;
                }
                
                animalInfo.AddFriendshipPoint(animal.fp);
                animalInfo.SetGetFriendshipRewards(animal.getFpRewards);
                
                AnimalInfoList.Add(animalInfo);
            }
        }

        public void SaveInfo()
        {
            if(AnimalInfoList == null)
                return;

            var jsonStr = JsonHelper.ToJson(AnimalInfoList.ToArray());
            var encodeStr = jsonStr.Encrypt(_secretKey);

            System.IO.File.WriteAllText(JsonFilePath, encodeStr);
        }

        public void SetPos(int id, Vector3 pos)
        {
            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return;

            if (!animalInfo.Arrangement)
                return;

            animalInfo.Pos = pos;
        }

        public bool AddAnimalInfo(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return false;

            if (GetAnimalInfo(animalInfo.Id) != null)
                return false;
            
            AnimalInfoList.Add(animalInfo);

            AddSkin(animalInfo, Games.Data.Const.AnimalBaseSkinId);
            
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
            animalInfo.SkinId = Games.Data.Const.AnimalBaseSkinId;

            SaveInfo();
        }

        public void ArrangeAnimal(int id, Vector3 pos)
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

        // 배치 된 경우, 변경된 스킨 적용.
        public void ApplySkin(int id, int skinId)
        {
            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return;

            animalInfo.SkinId = skinId;

            SaveInfo();
        }

        public bool CheckExist(int id, int skinId)
        {
            var animalInfo = GetAnimalInfo(id);
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
        
        #region Friendship
        public bool AddFriendshipPoint(int id, int point)
        {
            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return false;

            animalInfo.AddFriendshipPoint(point);

            return true;
        }

        public bool GetFriendshipReward(int id, int index)
        {
            var animalInfo = GetAnimalInfo(id);
            if (animalInfo == null)
                return false;

            return animalInfo.GetFriendshipReward(index);
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

