using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ScriptableObject/Const")]
public class Const : ScriptableObject
{
    [System.Serializable]
    public class CurrencyInfo
    {
        public Game.Type.EAnimalCurrency Animal = Game.Type.EAnimalCurrency.None;
        public Game.Type.EObjectCurrency Object = Game.Type.EObjectCurrency.None;
        public Info.User.Currency StartValue = null;

        public string AnimalSpriteName { get { return Animal.ToString().ToLower(); } }
        public string ObjectSpriteName { get { return Object.ToString().ToLower(); } }

        public int PlaceId
        {
            get
            {
                return StartValue != null ? StartValue.PlaceId : 0;
            }
        }
    }

    [SerializeField]
    private int animalBaseSkinId = 1;
    [SerializeField]
    private int startPlaceId = 1;
    [SerializeField]
    private int totalPlaceCount = 1;

    public int AnimalBaseSkinId { get { return animalBaseSkinId; } }
    public int StartPlaceId { get { return startPlaceId; } }
    public int TotalPlaceCount { get { return totalPlaceCount; } }

    public CurrencyInfo[] CurrencyInfos = null;

    public Info.User.Currency GetStartCurrency(int placeId)
    {
        if (CurrencyInfos == null)
        {
            return Info.User.GetInitializeCurrency(placeId);
        }

        foreach (var currencyInfo in CurrencyInfos)
        {
            if (currencyInfo == null)
                continue;

            if (currencyInfo.PlaceId == placeId)
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

        foreach (var currencyInfo in CurrencyInfos)
        {
            if (currencyInfo == null)
                continue;

            if (currencyInfo.PlaceId == placeId)
            {
                return currencyInfo;
            }
        }

        return null;
    }
}
