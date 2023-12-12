using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ScriptableObject/Const")]
public class Const : ScriptableObject
{
    [System.Serializable]
    public class PlaceData
    {
        public Game.Type.EPlaceName ePlaceName = Game.Type.EPlaceName.None;

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

    public int AnimalBaseSkinId { get { return animalBaseSkinId; } }
    public int StartPlaceId { get { return startPlaceId; } }
    public int TotalPlaceCount { get { return PlaceDatas != null ? PlaceDatas.Length : 1; } }

    public PlaceData[] PlaceDatas = null;

    public Info.User.Currency GetStartCurrency(int placeId)
    {
        if (PlaceDatas == null)
        {
            return Info.User.GetInitializeCurrency(placeId);
        }

        foreach (var data in PlaceDatas)
        {
            if (data == null)
                continue;

            if (data.PlaceId == placeId)
            {
                if(Application.isEditor)
                {
                    data.StartValue.Animal = 999999;
                    data.StartValue.Object = 999999;
                }

                return data.StartValue;
            }
        }

        return Info.User.GetInitializeCurrency(placeId);
    }

    public PlaceData GetPlaceData(int placeId)
    {
        if (PlaceDatas == null)
            return null;

        foreach (var data in PlaceDatas)
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

    public PlaceData ActivityPlaceData
    {
        get
        {
            int placeId = GameUtils.ActivityPlaceId;

            return GetPlaceData(placeId);
        }
    }
}
