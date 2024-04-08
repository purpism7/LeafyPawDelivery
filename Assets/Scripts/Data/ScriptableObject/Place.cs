using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(menuName = "Leafy Parcels/ScriptableObject/Data/Place")]
    public class Place : ScriptableObject
    {
        [System.Serializable]
        public class Data
        {
            [SerializeField]
            private int placeId = 0;
            public Game.Type.EPlaceName ePlaceName = Game.Type.EPlaceName.None;

            public Game.Type.EAnimalCurrency Animal = Game.Type.EAnimalCurrency.None;
            public Game.Type.EObjectCurrency Object = Game.Type.EObjectCurrency.None;
            //public Info.User.Currency StartValue = null;
            public float animalCurrencyRate = 1f;
            public float objectCurrencyRate = 1f;

            public string AnimalSpriteName { get { return Animal.ToString().ToLower(); } }
            public string ObjectSpriteName { get { return Object.ToString().ToLower(); } }

            public int PlaceId
            {
                get
                {
                    return placeId;
                }
            }
        }

        //public Game.Type.EPlaceName ePlaceName = Game.Type.EPlaceName.None;

        //public Game.Type.EAnimalCurrency Animal = Game.Type.EAnimalCurrency.None;
        //public Game.Type.EObjectCurrency Object = Game.Type.EObjectCurrency.None;
        //public Info.User.Currency StartValue = null;
        //public float animalCurrencyRate = 1f;
        //public float objectCurrencyRate = 1f;

        //public string AnimalSpriteName { get { return Animal.ToString().ToLower(); } } 
        //public string ObjectSpriteName { get { return Object.ToString().ToLower(); } }

       

        [SerializeField]
        private Data[] datas = null;

        public Data[] Datas { get { return datas; } }
        public int TotalPlaceCount { get { return datas != null ? datas.Length : 1; } }

        public Data GetPlaceData(int placeId)
        {
            if (datas == null)
                return null;

            foreach (var data in datas)
            {
                if (data == null)
                    continue;

                if (data.PlaceId == placeId)
                {
                    return data;
                }
            }

            return null;
        }

        public int LastPlaceId
        {
            get
            {
                int placeId = 0;

                if (datas == null)
                    return placeId;

                foreach (var data in datas)
                {
                    if (data == null)
                        continue;

                    if (data.PlaceId > placeId)
                    {
                        placeId = data.PlaceId;
                    }
                }

                return placeId;
            }
        }

//        public Info.User.Currency GetStartCurrency(int placeId)
//        {
//            var data = GetPlaceData(placeId);
//            if (data == null)
//                return null;

//#if UNITY_EDITOR
//            if (Application.isEditor)
//            {
//                return new Info.User.Currency()
//                {
//                    Animal = 999999,
//                    Object = 999999,
//                    PlaceId = placeId,
//                };
//            }
//#endif

//            return data.StartValue;
//        }
    }
}
