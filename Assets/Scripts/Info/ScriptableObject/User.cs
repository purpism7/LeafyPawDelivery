using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    [CreateAssetMenu(menuName = "Animals/ScriptableObject/User")]
    public class User : ScriptableObject
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

        // public List<int> AnimalIdList = new();
        // public List<int> ObjectIdList = new();
        // public List<int> PlaceIdList = new();

        public Currency GetCurrency(int placeId)
        {
            if (CurrencyList == null)
            {
                return new Currency()
                {
                    PlaceId = placeId,
                    Animal = 0,
                    Object = 0,
                };
            }
            
            return CurrencyList.Find(currency => currency.PlaceId == placeId);
        }
    }
}

