using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Info
{
    public class ObjectHolder : Holder.Base
    {
        private readonly string ObjectInfoJsonFilePath = "Assets/Info/Object.json";
        private readonly string ObjectUIdKey = "Object_UId_Key";

        public List<Info.Object> ObjectInfoList { get; private set; } = new();

        protected override void LoadInfo()
        {
            ObjectInfoList.Clear();

            var filePath = ObjectInfoJsonFilePath;
            if (!System.IO.File.Exists(filePath))
            {
                return;
            }

            var jsonString = System.IO.File.ReadAllText(filePath);
            var objectInfos = JsonHelper.FromJson<Info.Object>(jsonString);
            if(objectInfos != null)
            {
                ObjectInfoList.AddRange(objectInfos);
            }
        }

        private void SaveInfo()
        {
            if(ObjectInfoList == null)
            {
                return;
            }

            var jsonString = JsonHelper.ToJson(ObjectInfoList.ToArray());
            Debug.Log(jsonString);
            var filePath = ObjectInfoJsonFilePath;
            System.IO.File.WriteAllText(filePath, jsonString);
        }

        public void AddObject(Info.Object objectInfo)
        {
            if (objectInfo == null)
            {
                return;
            }

            int objectUId = PlayerPrefs.GetInt(ObjectUIdKey, 0);
            objectInfo.UId = ++objectUId;

            ObjectInfoList.Add(objectInfo);

            PlayerPrefs.SetInt(ObjectUIdKey, objectUId);

            SaveInfo();
        }

        public void ArrangeObject(int objectUId, Vector3 pos, int placeId)
        {
            var objectInfo = GetObjectInfo(objectUId);
            if(objectInfo == null)
            {
                return;
            }

            objectInfo.Pos = pos;
            objectInfo.PlaceId = placeId;

            SaveInfo();
        }

        public Info.Object GetObjectInfo(int objectUId)
        {
            if(ObjectInfoList == null)
            {
                return null;
            }

            return ObjectInfoList.Find(objectInfo => objectInfo.UId == objectUId);
        }
    }
}

