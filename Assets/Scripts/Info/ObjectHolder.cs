using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

using Game;

namespace Info
{
    public class ObjectHolder : Holder.Base
    {
        private readonly string ObjectUIdKey = "Object_UId_Key";
        
        protected override string JsonFilePath => RootJsonFilePath + "/Info/Object_Place_{0}.json";
        
        //public List<Info.Object> ObjectInfoList { get; private set; } = new();

        private Dictionary<int, List<Info.Object>> _objectInfoDic = new();

        public override void LoadInfo()
        {
            //ObjectInfoList.Clear();

            _objectInfoDic.Clear();

            for(int i = 1; i < 10; ++i)
            {
                var jsonfilePath = string.Format(JsonFilePath, i);
                if (!System.IO.File.Exists(jsonfilePath))
                    continue;

                var jsonString = System.IO.File.ReadAllText(jsonfilePath);
                var objectInfos = JsonHelper.FromJson<Info.Object>(jsonString);

                if (objectInfos == null)
                    continue;

                foreach (var objectInfo in objectInfos)
                {
                    if (objectInfo == null)
                        continue;

                    var objectData = ObjectContainer.Instance.GetData(objectInfo.Id);
                    if (objectData == null)
                        continue;

                    AddObject(objectData.PlaceId, objectInfo);
                }
            }
        }

        private void SaveInfo(int placeId)
        {
            if(_objectInfoDic == null)
                return;

            if(_objectInfoDic.TryGetValue(placeId, out List<Object> objectInfoList))
            {
                if (objectInfoList == null ||
                    objectInfoList.Count <= 0)
                    return;

                var jsonString = JsonHelper.ToJson(objectInfoList.ToArray());
                
                System.IO.File.WriteAllText(JsonFilePath, jsonString);
            }
        }

        public void AddObject(Info.Object objectInfo)
        {
            if (objectInfo == null)
                return;

            var objectData = ObjectContainer.Instance.GetData(objectInfo.Id);
            if (objectData == null)
                return;

            int objectUId = PlayerPrefs.GetInt(ObjectUIdKey, 0);
            objectInfo.UId = ++objectUId;

            AddObject(objectData.PlaceId, objectInfo);

            PlayerPrefs.SetInt(ObjectUIdKey, objectUId);

            SaveInfo(objectData.PlaceId);
        }

        private void AddObject(int placeId, Object objectInfo)
        {
            List<Object> objectInfoList = null;
            if (_objectInfoDic.TryGetValue(placeId, out objectInfoList))
            {
                objectInfoList.Add(objectInfo);
            }
            else
            {
                objectInfoList = new List<Object>();
                objectInfoList.Clear();
                objectInfoList.Add(objectInfo);

                _objectInfoDic.Add(placeId, objectInfoList);
            }
        }

        public void RemoveObject(int objectId, int objectUId)
        {
            var objectData = ObjectContainer.Instance.GetData(objectId);
            if (objectData == null)
                return;

            var objectInfo = GetObjectInfoByUId(objectUId, objectData.PlaceId);
            if (objectInfo == null)
                return;
            
            objectInfo.Pos = Vector3.zero;
            objectInfo.Arrangement = false;

            SaveInfo(objectData.PlaceId);
        }

        public void ArrangeObject(int objectUId, Vector3 pos, int placeId)
        {
            var objectInfo = GetObjectInfoByUId(objectUId, placeId);
            if(objectInfo == null)
            {
                return;
            }

            objectInfo.Pos = pos;
            objectInfo.Arrangement = true;

            SaveInfo(placeId);
        }

        public Info.Object GetObjectInfoByUId(int objectUId, int placeId)
        {
            if(_objectInfoDic == null)
                return null;

            var objectInfoList = GetObjectInfoList(placeId);
            if (objectInfoList == null)
                return null;
            
            return objectInfoList.Find(objectInfo => objectInfo.UId == objectUId);
        }

        public Info.Object GetObjectInfoById(int objectId, int placeId)
        {
            if(_objectInfoDic == null)
                return null;

            var objectInfoList = GetObjectInfoList(placeId);
            if (objectInfoList == null)
                return null;
            
            return objectInfoList.Find(objectInfo => objectInfo.Id == objectId);
        }

        public List<Info.Object> GetObjectInfoList(int placeId)
        {
            if (_objectInfoDic == null)
                return null;

            if (_objectInfoDic.TryGetValue(placeId, out List<Object> objectInfoList))
            {
                return objectInfoList;
            }

            return null;
        }
    }
}

