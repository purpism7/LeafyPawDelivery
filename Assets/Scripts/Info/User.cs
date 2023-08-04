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

        [Serializable]
        public class Story
        {
            public int PlaceId = 0;
            public int StoryId = 0;
        }

        public List<Currency> CurrencyList = new();
        public List<Story> StoryList = new();

        #region Currency
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

        public void SetAnimalCurrency(int placeId, int value)
        {
            if (CurrencyList == null)
            {
                CurrencyList = new();
                CurrencyList.Clear();
            }

            int findIndex = CurrencyList.FindIndex(findCurrency => findCurrency.PlaceId == placeId);
            if (findIndex >= 0)
            {
                CurrencyList[findIndex].Animal += value;
            }
        }

        public void SetObjectCurrency(int placeId, int value)
        {
            if(CurrencyList == null)
            {
                CurrencyList = new();
                CurrencyList.Clear();
            }

            int findIndex = CurrencyList.FindIndex(findCurrency => findCurrency.PlaceId == placeId);
            if(findIndex >= 0)
            {
                CurrencyList[findIndex].Object += value;
            }
        }

        public void SetCurrency(Currency currency)
        {
            if (currency == null)
                return;

            if (CurrencyList == null)
            {
                CurrencyList = new();
                CurrencyList.Clear();
            }

            int findIndex = CurrencyList.FindIndex(findCurrency => findCurrency.PlaceId == currency.PlaceId);
            if (findIndex >= 0)
            {
                CurrencyList[findIndex].Animal += currency.Animal;
                CurrencyList[findIndex].Object += currency.Object;
            }
        }

        public Currency CurrenctCurrency
        {
            get
            {
                int placeId = MainGameManager.Instance.placeMgr.ActivityPlaceId;

                return GetCurrency(placeId);
            }
        }
        #endregion

        #region Story
        public Story GetStory(int placeId)
        {
            if (StoryList == null)
            {
                return new Story()
                {
                    PlaceId = placeId,
                    StoryId = 0,
                };
            }

            return StoryList.Find(story => story.PlaceId == placeId);
        }

        public void SetStory(int placeId, int storyId)
        {
            if(StoryList == null)
                return;

            var findIndex = StoryList.FindIndex(story => story.PlaceId == placeId);
            if(findIndex >= 0)
            {
                StoryList[findIndex].StoryId = storyId;
            }
            else
            {
                StoryList.Add(new Story()
                {
                    PlaceId = placeId,
                    StoryId = storyId,
                });
            }
        }
        #endregion
    }
}

