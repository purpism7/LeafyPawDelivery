using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class User
    {
        public long Cash = 0;
        
        [Serializable]
        public class Currency
        {
            public int PlaceId = 0;
            public long Animal = 0;
            public long Object = 0;
        }

        public List<Currency> CurrencyList = new();
        public List<int> StoryList = new();


        public static Currency GetInitializeCurrency(int placeId)
        {
            return new Currency()
            {
                PlaceId = placeId,
                Animal = 0,
                Object = 0,
            };
        }

        public Currency GetCurrency(int placeId)
        {
            if (CurrencyList == null)
            {
                return GetInitializeCurrency(placeId);
            }
            
            return CurrencyList.Find(currency => currency.PlaceId == placeId);
        }

        public Currency CurrenctCurrency
        {
            get
            {
                int placeId = MainGameManager.Instance.placeMgr.ActivityPlaceId;

                return GetCurrency(placeId);
            }
        }
    }
}

