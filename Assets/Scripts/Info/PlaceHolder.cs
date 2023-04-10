using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class PlaceHolder
    {
        readonly private string PlaceInfoJsonFilePath = "Assets/Info/Place_{0}.json";

        private Dictionary<int, Info.Place> _placeDic = new();

        public PlaceHolder()
        {
            _placeDic.Clear();
        }

        public void Init(List<int> placeIdList)
        {
            LoadPlaceList(placeIdList);
        }

        /// <summary>
        /// 배치한 오브젝트를 Dictionary 에 추가 및 수정.
        /// /// </summary>
        /// <param name="placeId"></param>
        /// <param name="obj"></param>
        public void AddObject(int placeId, Info.Object obj)
        {
            if(obj == null)
            {
                return;
            }

            if(_placeDic.TryGetValue(placeId, out Info.Place place))
            {
                if(place != null)
                {
                    place.AddObject(obj);
                }
            }
            else
            {
                _placeDic.Add(placeId, new Info.Place(placeId, new List<Object>() { obj, }));
            }
        }

        private void LoadPlaceList(List<int> placeIdList)
        {
            if(placeIdList == null)
            {
                return;
            }

            foreach(var placeId in placeIdList)
            {
                LoadInfo(placeId);
            }
        }

        private void LoadInfo(int placeId)
        {
            var filePath = string.Format(PlaceInfoJsonFilePath, placeId);
            if(!System.IO.File.Exists(filePath))
            {
                return;
            }

            var jsonString = System.IO.File.ReadAllText(filePath);

            var place = JsonUtility.FromJson<Info.Place>(jsonString);
            if(place == null)
            {
                return;
            }

            _placeDic.Add(placeId, place);
        }

        private void SaveInfo(int placeId)
        {
            if(_placeDic.TryGetValue(placeId, out Info.Place place))
            {
                var jsonString = JsonUtility.ToJson(place);

                var filePath = string.Format(PlaceInfoJsonFilePath, placeId);
                System.IO.File.WriteAllText(filePath, jsonString);
            }
        }
    }
}

