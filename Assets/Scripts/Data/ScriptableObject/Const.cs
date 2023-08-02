using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(menuName = "Game/ScriptableObject/Const")]
    public class Const : ScriptableObject
    {
        public int StartPlaceId = 1;
        public Info.User.Currency[] StartCurrencies = null;

        public Info.User.Currency GetStartCurrency(int placeId)
        {
            if(StartCurrencies == null)
            {
                return Info.User.GetInitializeCurrency(placeId);
            }

            foreach(var currency in StartCurrencies)
            {
                if (currency == null)
                    continue;

                if(currency.PlaceId == placeId)
                {
                    return currency;
                }
            }

            return Info.User.GetInitializeCurrency(placeId);
        }
    }
}
