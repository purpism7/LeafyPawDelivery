using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class User
    {
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

        public long Cash = 0;

        [SerializeField]
        private int lastPlaceId = 1;
        
        public List<Currency> CurrencyList = new();
        public List<Story> StoryList = new();

        public int LastPlaceId { get { return lastPlaceId; } }

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

        private Currency GetStartCurrency(int placeId)
        {
            return Game.Data.Const.GetStartCurrency(placeId);
        }

        private Currency InitializeCurrency(int placeId)
        {
            CurrencyList = new();
            CurrencyList.Clear();

            var currency = GetStartCurrency(placeId);
            if (currency == null)
            {
                currency = GetInitializeCurrency(placeId);
            }

            CurrencyList.Add(currency);

            return currency;
        }

        public Currency GetCurrency(int placeId)
        {
            if (CurrencyList == null)
            {
                return InitializeCurrency(placeId);
            }

            var findCurrency = CurrencyList.Find(currency => currency.PlaceId == placeId);
            if (findCurrency == null)
            {
                var currency = GetStartCurrency(placeId);
                if (currency == null)
                {
                    currency = GetInitializeCurrency(placeId);
                }

                CurrencyList.Add(currency);

                return currency;
            }

            return findCurrency;
        }

        public void SetCash(long value)
        {
            Cash += value;
        }

        public void SetAnimalCurrency(int placeId, int value)
        {
            if (CurrencyList == null)
            {
                InitializeCurrency(placeId);
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
                InitializeCurrency(placeId);
            }

            int findIndex = CurrencyList.FindIndex(findCurrency => findCurrency.PlaceId == placeId);
            if(findIndex >= 0)
            {
                CurrencyList[findIndex].Object += value;
            }
        }

        public void SetCurrency(Game.Type.EElement eElement, int currency)
        {
            int placeId = CurrentPlaceId;
            if(eElement == Game.Type.EElement.Animal)
            {
                SetAnimalCurrency(placeId, currency);
            }
            else if(eElement == Game.Type.EElement.Object)
            {
                SetObjectCurrency(placeId, currency);
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
            else
            {
                CurrencyList.Add(currency);
            }
        }

        public Currency CurrenctCurrency
        {
            get
            {
                return GetCurrency(CurrentPlaceId);
            }
        }

        private int CurrentPlaceId
        {
            get
            {
                var placeMgr = MainGameManager.Get<Game.PlaceManager>();
                if (placeMgr == null)
                    return 0;

                return placeMgr.ActivityPlaceId;
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

        public int GetLastStoryId(int placeId)
        {
            var story = GetStory(placeId);
            if (story == null)
                return 0;

            return story.StoryId;
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

        public void SetLastPlaceId(int id)
        {
            if (lastPlaceId >= id)
                return;

            lastPlaceId = id;
        }
    }
}

