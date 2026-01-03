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


        // public Game.Type.EObjectGrade EGrade = Game.Type.EObjectGrade.None;
        public string ShortIconImgName = string.Empty;
        public string LargeIconImgName = string.Empty;

        public override Game.Type.EElement EElement => Game.Type.EElement.Object;

        // public int Order { get { return order; } }

        public override void Initialize()
        {
            base.Initialize();

            // System.Enum.TryParse(Grade.ToString(), out EGrade);

            if (Count <= 0)
            {
                Count = (int)Grade;
                if (Grade == Game.Type.EObjectGrade.None)
                {
                    Count = 1;
                }
            }

            string placeId = PlaceId > 9 ? PlaceId.ToString() : "0" + PlaceId;
            string id = Id > 9 ? Id.ToString() : "0" + Id;

            ShortIconImgName = string.Format("EditIcon_Map{0}_Object_{1}", placeId, id);
            LargeIconImgName = string.Format("BookIcon_Map{0}_Object_{1}", placeId, id);
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

