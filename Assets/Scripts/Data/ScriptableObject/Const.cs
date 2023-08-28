using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(menuName = "Game/ScriptableObject/Const")]
    public class Const : ScriptableObject
    {
        [System.Serializable]
        public class CurrencyInfo
        {
            public Game.Type.EAnimalCurrency Animal = Game.Type.EAnimalCurrency.None;
            public Game.Type.EObjectCurrency Object = Game.Type.EObjectCurrency.None;
            public Info.User.Currency StartValue = null;
           
            public int PlaceId
            {
                get
                {
                    return StartValue != null ? StartValue.PlaceId : 0;
                }
            }
        }

        public int StartPlaceId = 1;
        public CurrencyInfo[] CurrencyInfos = null;

        public Info.User.Currency GetStartCurrency(int placeId)
        {
            if(CurrencyInfos == null)
            {
                return Info.User.GetInitializeCurrency(placeId);
            }

            foreach(var currencyInfo in CurrencyInfos)
            {
                if (currencyInfo == null)
                    continue;

                if(currencyInfo.PlaceId == placeId)
                {
                    return currencyInfo.StartValue;
                }
            }

            return Info.User.GetInitializeCurrency(placeId);
        }

        public CurrencyInfo GetCurrencyInfo(int placeId)
        {
            if (CurrencyInfos == null)
                return null;

            foreach(var currencyInfo in CurrencyInfos)
            {
                if (currencyInfo == null)
                    continue;

                if(currencyInfo.PlaceId == placeId)
                {
                    return currencyInfo;
                }
            }

            return null;
        }
    }
}
