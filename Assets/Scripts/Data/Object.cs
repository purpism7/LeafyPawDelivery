using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

using Newtonsoft;
using Newtonsoft.Json;

namespace Data
{
    [System.Serializable]
    public class Object : ElementData
    {
        [JsonProperty("type")] public Type.ObjectType ObjectType { get; private set; } = Type.ObjectType.None;
        [JsonProperty("Count")] public int Count { get; private set; } = 0;
        [JsonProperty("object_grade")] public Type.EObjectGrade Grade { get; private set; } = Type.EObjectGrade.None;
        [JsonProperty("order")] public int Order { get; private set; } = 0;

        public string ShortIconImgName = string.Empty;
        public string LargeIconImgName = string.Empty;

        public override Game.Type.EElement EElement => Game.Type.EElement.Object;

        public override void Initialize()
        {
            base.Initialize();

            if (Count <= 0)
            {
                Count = (int)Grade;
                if (Grade == Game.Type.EObjectGrade.None)
                {
                    Count = 1;
                }
            }

            string placeId = PlaceId <= 0 ? string.Empty : PlaceId.ToString("D2");
            string id = Id.ToString("D2");

            ShortIconImgName = $"EditIcon_Map{placeId}_Object_{id}";
            LargeIconImgName = $"BookIcon_Map{placeId}_Object_{id}";
        }

        public override int Currency
        {
            get
            {
                switch (Grade)
                {
                    case Game.Type.EObjectGrade.Unique:
                        return 50;

                    case Game.Type.EObjectGrade.Epic:
                        return 40;

                    case Game.Type.EObjectGrade.Rare:
                        return 35;

                    case Game.Type.EObjectGrade.Normal:
                        return 23;

                    case Game.Type.EObjectGrade.Special:
                        return 50;
                            

                    default:
                        return 0;
                }
            }
        }
    }
}

